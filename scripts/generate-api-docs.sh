#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
PROJECT_FILE="$ROOT_DIR/src/DbExport.Api/DbExport.Api.csproj"
OUT_DIR="$ROOT_DIR/docs"
OUT_FILE="$OUT_DIR/DbExport.Api.md"

mkdir -p "$OUT_DIR"

TMP_DIR="$(mktemp -d)"
cleanup() {
  rm -rf "$TMP_DIR"
}
trap cleanup EXIT

XML_FILE="$TMP_DIR/DbExport.Api.xml"
ALLOWLIST_FILE="$TMP_DIR/DbExport.Api.allowlist.txt"
FILTER_PROJECT_DIR="$TMP_DIR/allowlist-gen"

dotnet build "$PROJECT_FILE" \
  -nologo \
  -v minimal \
  -p:GenerateDocumentationFile=true \
  -p:DocumentationFile="$XML_FILE" \
  >/dev/null

if [[ ! -f "$XML_FILE" ]]; then
  echo "Failed to generate XML documentation file: $XML_FILE" >&2
  exit 1
fi

PROJECT_DIR="$(cd "$(dirname "$PROJECT_FILE")" && pwd)"
SEARCH_DIRS=()
if [[ -d "$PROJECT_DIR/bin" ]]; then
  SEARCH_DIRS+=("$PROJECT_DIR/bin")
fi
if [[ -d "$ROOT_DIR/bin" ]]; then
  SEARCH_DIRS+=("$ROOT_DIR/bin")
fi

ASSEMBLY_FILE=""
if [[ ${#SEARCH_DIRS[@]} -gt 0 ]]; then
  ASSEMBLY_FILE="$(find "${SEARCH_DIRS[@]}" -type f -name 'DbExport.Api.dll' ! -path '*/ref/*' | head -n 1)"
fi
if [[ -z "${ASSEMBLY_FILE:-}" || ! -f "$ASSEMBLY_FILE" ]]; then
  echo "Failed to locate built assembly for reflection filtering." >&2
  exit 1
fi

mkdir -p "$FILTER_PROJECT_DIR"

cat > "$FILTER_PROJECT_DIR/AllowlistGen.csproj" <<'XML'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>disable</Nullable>
  </PropertyGroup>
</Project>
XML

cat > "$FILTER_PROJECT_DIR/Program.cs" <<'CS'
using System.Reflection;

if (args.Length < 2)
{
    Console.Error.WriteLine("Expected: <assemblyPath> <outputPath>");
    return 1;
}

var assemblyPath = args[0];
var outputPath = args[1];
var asm = Assembly.LoadFrom(assemblyPath);
var ids = new HashSet<string>(StringComparer.Ordinal);

const BindingFlags allDeclared = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

foreach (var type in asm.GetTypes())
{
    if (IsExcludedNamespace(type.Namespace) || type.IsNestedPrivate || type.Name.StartsWith("<", StringComparison.Ordinal))
    {
        continue;
    }

    ids.Add("T:" + TypeDefinitionId(type));

    foreach (var field in type.GetFields(allDeclared))
    {
        if (field.IsPrivate || field.Name.StartsWith("<", StringComparison.Ordinal))
        {
            continue;
        }
        ids.Add("F:" + TypeDefinitionId(type) + "." + field.Name);
    }

    foreach (var prop in type.GetProperties(allDeclared))
    {
        var accessor = prop.GetMethod ?? prop.SetMethod;
        if (accessor == null || accessor.IsPrivate || prop.Name.StartsWith("<", StringComparison.Ordinal))
        {
            continue;
        }
        var indexer = BuildParameterList(prop.GetIndexParameters().Select(p => p.ParameterType));
        ids.Add("P:" + TypeDefinitionId(type) + "." + prop.Name + indexer);
    }

    foreach (var ev in type.GetEvents(allDeclared))
    {
        var accessor = ev.AddMethod ?? ev.RemoveMethod ?? ev.RaiseMethod;
        if (accessor == null || accessor.IsPrivate || ev.Name.StartsWith("<", StringComparison.Ordinal))
        {
            continue;
        }
        ids.Add("E:" + TypeDefinitionId(type) + "." + ev.Name);
    }

    foreach (var ctor in type.GetConstructors(allDeclared))
    {
        if (ctor.IsPrivate)
        {
            continue;
        }
        ids.Add("M:" + TypeDefinitionId(type) + ".#ctor" + BuildParameterList(ctor.GetParameters().Select(p => p.ParameterType)));
    }

    foreach (var method in type.GetMethods(allDeclared))
    {
        if (method.IsPrivate || method.IsSpecialName || method.Name.StartsWith("<", StringComparison.Ordinal))
        {
            continue;
        }

        var methodName = method.Name;
        if (method.IsGenericMethodDefinition)
        {
            methodName += "``" + method.GetGenericArguments().Length;
        }

        ids.Add("M:" + TypeDefinitionId(type) + "." + methodName + BuildParameterList(method.GetParameters().Select(p => p.ParameterType)));
    }
}

File.WriteAllLines(outputPath, ids.OrderBy(x => x, StringComparer.Ordinal));
return 0;

static bool IsExcludedNamespace(string ns) => ns != null && ns.StartsWith("System.", StringComparison.Ordinal);

static string BuildParameterList(IEnumerable<Type> parameterTypes)
{
    var parts = parameterTypes.Select(TypeReferenceId).ToArray();
    return parts.Length == 0 ? string.Empty : "(" + string.Join(",", parts) + ")";
}

static string TypeDefinitionId(Type type)
{
    var segments = new Stack<string>();
    var current = type;
    while (current != null)
    {
        segments.Push(current.Name);
        current = current.DeclaringType;
    }

    var ns = type.Namespace;
    return (string.IsNullOrEmpty(ns) ? string.Empty : ns + ".") + string.Join(".", segments);
}

static string TypeReferenceId(Type type)
{
    if (type.IsGenericParameter)
    {
        return type.DeclaringMethod != null ? "``" + type.GenericParameterPosition : "`" + type.GenericParameterPosition;
    }

    if (type.IsByRef)
    {
        return TypeReferenceId(type.GetElementType()) + "@";
    }

    if (type.IsPointer)
    {
        return TypeReferenceId(type.GetElementType()) + "*";
    }

    if (type.IsArray)
    {
        var rank = type.GetArrayRank();
        var suffix = rank == 1 ? "[]" : "[" + new string(',', rank - 1) + "]";
        return TypeReferenceId(type.GetElementType()) + suffix;
    }

    if (type.IsGenericType && !type.IsGenericTypeDefinition)
    {
        var genericDef = type.GetGenericTypeDefinition();
        var baseName = TypeNameWithoutArity(genericDef);
        var args = type.GetGenericArguments().Select(TypeReferenceId);
        return baseName + "{" + string.Join(",", args) + "}";
    }

    return TypeDefinitionId(type);
}

static string TypeNameWithoutArity(Type type)
{
    var segments = new Stack<string>();
    var current = type;
    while (current != null)
    {
        var name = current.Name;
        var tick = name.IndexOf('`');
        segments.Push(tick >= 0 ? name.Substring(0, tick) : name);
        current = current.DeclaringType;
    }

    var ns = type.Namespace;
    return (string.IsNullOrEmpty(ns) ? string.Empty : ns + ".") + string.Join(".", segments);
}
CS

dotnet run --project "$FILTER_PROJECT_DIR/AllowlistGen.csproj" -- "$ASSEMBLY_FILE" "$ALLOWLIST_FILE" >/dev/null

awk -v out_file="$OUT_FILE" -v allow_file="$ALLOWLIST_FILE" '
  function trim(s) {
    gsub(/^[[:space:]]+|[[:space:]]+$/, "", s)
    return s
  }

  function normalize_ref(ref,    r) {
    r = ref
    sub(/^.*:/, "", r)
    gsub("{", "<", r)
    gsub("}", ">", r)
    return r
  }

  function clean_inline_tags(s,    t, tag, ref, word, name) {
    t = s

    while (match(t, /<see[[:space:]]+cref="[^"]+"[[:space:]]*\/>/)) {
      tag = substr(t, RSTART, RLENGTH)
      ref = tag
      sub(/^.*cref="/, "", ref)
      sub(/"[[:space:]]*\/>$/, "", ref)
      ref = normalize_ref(ref)
      t = substr(t, 1, RSTART - 1) ref substr(t, RSTART + RLENGTH)
    }

    while (match(t, /<see[[:space:]]+langword="[^"]+"[[:space:]]*\/>/)) {
      tag = substr(t, RSTART, RLENGTH)
      word = tag
      sub(/^.*langword="/, "", word)
      sub(/"[[:space:]]*\/>$/, "", word)
      t = substr(t, 1, RSTART - 1) word substr(t, RSTART + RLENGTH)
    }

    while (match(t, /<paramref[[:space:]]+name="[^"]+"[[:space:]]*\/>/)) {
      tag = substr(t, RSTART, RLENGTH)
      name = tag
      sub(/^.*name="/, "", name)
      sub(/"[[:space:]]*\/>$/, "", name)
      t = substr(t, 1, RSTART - 1) name substr(t, RSTART + RLENGTH)
    }

    while (match(t, /<typeparamref[[:space:]]+name="[^"]+"[[:space:]]*\/>/)) {
      tag = substr(t, RSTART, RLENGTH)
      name = tag
      sub(/^.*name="/, "", name)
      sub(/"[[:space:]]*\/>$/, "", name)
      t = substr(t, 1, RSTART - 1) name substr(t, RSTART + RLENGTH)
    }

    while (match(t, /<c[[:space:]]+langword="[^"]+"[[:space:]]*\/>/)) {
      tag = substr(t, RSTART, RLENGTH)
      word = tag
      sub(/^.*langword="/, "", word)
      sub(/"[[:space:]]*\/>$/, "", word)
      t = substr(t, 1, RSTART - 1) word substr(t, RSTART + RLENGTH)
    }

    gsub(/<c>/, "`", t)
    gsub(/<\/c>/, "`", t)
    gsub(/<code>/, "`", t)
    gsub(/<\/code>/, "`", t)
    gsub(/<para>/, " ", t)
    gsub(/<\/para>/, " ", t)

    gsub(/<[^>]+>/, "", t)
    gsub(/&lt;/, "<", t)
    gsub(/&gt;/, ">", t)
    gsub(/&amp;/, "\\&", t)
    gsub(/&quot;/, "\"", t)
    gsub(/&apos;/, "\047", t)
    gsub(/[[:space:]]+/, " ", t)
    return trim(t)
  }

  function append_text(existing, addition) {
    if (length(addition) == 0) return existing
    if (length(existing) == 0) return addition
    return existing " " addition
  }

  function reset_doc_state() {
    doc_summary = ""
    doc_returns = ""
    doc_param_count = 0
    doc_tparam_count = 0
    doc_exception_count = 0
    delete doc_param_order
    delete doc_param_text
    delete doc_tparam_order
    delete doc_tparam_text
    delete doc_exception_order
    delete doc_exception_text
    mode = ""
    mode_key = ""
  }

  function store_param(name, text) {
    if (!(name in doc_param_text)) {
      doc_param_count++
      doc_param_order[doc_param_count] = name
      doc_param_text[name] = ""
    }
    doc_param_text[name] = append_text(doc_param_text[name], text)
  }

  function store_tparam(name, text) {
    if (!(name in doc_tparam_text)) {
      doc_tparam_count++
      doc_tparam_order[doc_tparam_count] = name
      doc_tparam_text[name] = ""
    }
    doc_tparam_text[name] = append_text(doc_tparam_text[name], text)
  }

  function store_exception(exc, text) {
    if (!(exc in doc_exception_text)) {
      doc_exception_count++
      doc_exception_order[doc_exception_count] = exc
      doc_exception_text[exc] = ""
    }
    doc_exception_text[exc] = append_text(doc_exception_text[exc], text)
  }

  function parse_doc_line(raw,    line, text, name, exc, start, stop) {
    line = trim(raw)

    if (line ~ /^<summary>/) {
      mode = "summary"
      sub(/^<summary>/, "", line)
      if (line ~ /<\/summary>$/) {
        sub(/<\/summary>$/, "", line)
        text = clean_inline_tags(line)
        doc_summary = append_text(doc_summary, text)
        mode = ""
      } else {
        text = clean_inline_tags(line)
        doc_summary = append_text(doc_summary, text)
      }
      return
    }

    if (line ~ /^<\/summary>/) {
      mode = ""
      return
    }

    if (line ~ /^<returns>/) {
      mode = "returns"
      sub(/^<returns>/, "", line)
      if (line ~ /<\/returns>$/) {
        sub(/<\/returns>$/, "", line)
        text = clean_inline_tags(line)
        doc_returns = append_text(doc_returns, text)
        mode = ""
      } else {
        text = clean_inline_tags(line)
        doc_returns = append_text(doc_returns, text)
      }
      return
    }

    if (line ~ /^<\/returns>/) {
      mode = ""
      return
    }

    if (match(line, /^<param[[:space:]]+name="[^"]+">/)) {
      name = substr(line, RSTART, RLENGTH)
      sub(/^.*name="/, "", name)
      sub(/">$/, "", name)
      mode = "param"
      mode_key = name
      sub(/^<param[[:space:]]+name="[^"]+">/, "", line)
      if (line ~ /<\/param>$/) {
        sub(/<\/param>$/, "", line)
        text = clean_inline_tags(line)
        store_param(name, text)
        mode = ""
        mode_key = ""
      } else {
        text = clean_inline_tags(line)
        store_param(name, text)
      }
      return
    }

    if (line ~ /^<\/param>/) {
      mode = ""
      mode_key = ""
      return
    }

    if (match(line, /^<typeparam[[:space:]]+name="[^"]+">/)) {
      name = substr(line, RSTART, RLENGTH)
      sub(/^.*name="/, "", name)
      sub(/">$/, "", name)
      mode = "typeparam"
      mode_key = name
      sub(/^<typeparam[[:space:]]+name="[^"]+">/, "", line)
      if (line ~ /<\/typeparam>$/) {
        sub(/<\/typeparam>$/, "", line)
        text = clean_inline_tags(line)
        store_tparam(name, text)
        mode = ""
        mode_key = ""
      } else {
        text = clean_inline_tags(line)
        store_tparam(name, text)
      }
      return
    }

    if (line ~ /^<\/typeparam>/) {
      mode = ""
      mode_key = ""
      return
    }

    if (match(line, /^<exception[[:space:]]+cref="[^"]+">/)) {
      exc = substr(line, RSTART, RLENGTH)
      sub(/^.*cref="/, "", exc)
      sub(/">$/, "", exc)
      exc = normalize_ref(exc)
      mode = "exception"
      mode_key = exc
      sub(/^<exception[[:space:]]+cref="[^"]+">/, "", line)
      if (line ~ /<\/exception>$/) {
        sub(/<\/exception>$/, "", line)
        text = clean_inline_tags(line)
        store_exception(exc, text)
        mode = ""
        mode_key = ""
      } else {
        text = clean_inline_tags(line)
        store_exception(exc, text)
      }
      return
    }

    if (line ~ /^<\/exception>/) {
      mode = ""
      mode_key = ""
      return
    }

    text = clean_inline_tags(line)
    if (mode == "summary") {
      doc_summary = append_text(doc_summary, text)
    } else if (mode == "returns") {
      doc_returns = append_text(doc_returns, text)
    } else if (mode == "param") {
      store_param(mode_key, text)
    } else if (mode == "typeparam") {
      store_tparam(mode_key, text)
    } else if (mode == "exception") {
      store_exception(mode_key, text)
    }
  }

  function add_namespace(ns) {
    if (!(ns in seen_namespace)) {
      seen_namespace[ns] = 1
      namespace_count++
      namespace_order[namespace_count] = ns
    }
  }

  function add_type(ns, type_full, type_short, kind, summary,    key) {
    key = ns SUBSEP type_full
    if (!(key in seen_type)) {
      seen_type[key] = 1
      type_count++
      type_order[type_count] = key
      type_ns[key] = ns
      type_full_name[key] = type_full
      type_short_name[key] = type_short
      type_kind[key] = kind
      type_summary[key] = summary
    } else if (length(type_summary[key]) == 0 && length(summary) > 0) {
      type_summary[key] = summary
    }
  }

  function add_member(type_full, cat, signature, summary, returns,    mid, i, name, exc, key) {
    if (!(type_full in type_to_ns)) {
      ns = "(global)"
      split_type(type_full)
      ns = split_ns
      if (length(ns) == 0) ns = "(global)"
      add_namespace(ns)
      add_type(ns, type_full, split_type_short, "type", "")
      type_to_ns[type_full] = ns
    }

    mid = ++member_count
    member_type[mid] = type_full
    member_category[mid] = cat
    member_signature[mid] = signature
    member_summary[mid] = summary
    member_returns[mid] = returns

    member_param_count[mid] = doc_param_count
    for (i = 1; i <= doc_param_count; i++) {
      name = doc_param_order[i]
      member_param_name[mid, i] = name
      member_param_text[mid, i] = doc_param_text[name]
    }

    member_tparam_count[mid] = doc_tparam_count
    for (i = 1; i <= doc_tparam_count; i++) {
      name = doc_tparam_order[i]
      member_tparam_name[mid, i] = name
      member_tparam_text[mid, i] = doc_tparam_text[name]
    }

    member_exception_count[mid] = doc_exception_count
    for (i = 1; i <= doc_exception_count; i++) {
      exc = doc_exception_order[i]
      member_exception_name[mid, i] = exc
      member_exception_text[mid, i] = doc_exception_text[exc]
    }

    key = type_full SUBSEP cat
    if (!(key in member_list)) {
      member_list[key] = mid
    } else {
      member_list[key] = member_list[key] "," mid
    }
  }

  function split_type(full,    pos) {
    split_ns = ""
    split_type_short = full
    pos = last_dot(full)
    if (pos > 0) {
      split_ns = substr(full, 1, pos - 1)
      split_type_short = substr(full, pos + 1)
    }
  }

  function last_dot(s,    i) {
    for (i = length(s); i >= 1; i--) {
      if (substr(s, i, 1) == ".") return i
    }
    return 0
  }

  function get_declaring_type(member_name,    sig, pos) {
    sig = member_name
    if (index(sig, "(") > 0) {
      sig = substr(sig, 1, index(sig, "(") - 1)
    }
    pos = last_dot(sig)
    if (pos == 0) return ""
    return substr(sig, 1, pos - 1)
  }

  function get_member_simple_name(member_name,    sig, pos, n) {
    sig = member_name
    if (index(sig, "(") > 0) {
      sig = substr(sig, 1, index(sig, "(") - 1)
    }
    pos = last_dot(sig)
    if (pos == 0) return sig
    n = substr(sig, pos + 1)
    return n
  }

  function get_param_sig(member_name,    pos) {
    pos = index(member_name, "(")
    if (pos == 0) return ""
    return substr(member_name, pos)
  }

  function category_order_name(i) {
    if (i == 1) return "fields"
    if (i == 2) return "properties"
    if (i == 3) return "constructors"
    if (i == 4) return "methods"
    return "others"
  }

  function category_title(cat) {
    if (cat == "fields") return "Fields"
    if (cat == "properties") return "Properties"
    if (cat == "constructors") return "Constructors"
    if (cat == "methods") return "Methods"
    return "Other Members"
  }

  function anchor(s,    a) {
    a = tolower(s)
    gsub(/[^a-z0-9]+/, "-", a)
    gsub(/^-+|-+$/, "", a)
    return a
  }

  function display_type_name(s,    n) {
    n = s
    gsub(/`/, "", n)
    return n
  }

  function is_excluded_namespace(ns) {
    return (ns ~ /^System\./)
  }

  BEGIN {
    reset_doc_state()
    while ((getline line < allow_file) > 0) {
      sub(/\r$/, "", line)
      line = trim(line)
      if (length(line) > 0) {
        allow[line] = 1
      }
    }
    close(allow_file)
    RS = "</member>"
  }

  /<member[[:space:]]+name="[^"]+"/ {
    reset_doc_state()

    record = $0
    if (!match(record, /<member[[:space:]]+name="[^"]+"/)) next

    header = substr(record, RSTART, RLENGTH)
    full_name = header
    sub(/^.*name="/, "", full_name)
    sub(/"$/, "", full_name)

    if (!(full_name in allow)) next

    prefix = substr(full_name, 1, 1)
    item_name = substr(full_name, 3)

    line_count = split(record, lines, /\n/)
    for (i = 1; i <= line_count; i++) {
      l = lines[i]
      if (l ~ /<member[[:space:]]+name="/) continue
      parse_doc_line(l)
    }

    if (prefix == "T") {
      split_type(item_name)
      ns = split_ns
      if (length(ns) == 0) ns = "(global)"
      if (is_excluded_namespace(ns)) next
      add_namespace(ns)

      kind = "type"
      if (doc_summary ~ /\binterface\b/) kind = "interface"
      add_type(ns, item_name, split_type_short, kind, doc_summary)
      type_to_ns[item_name] = ns

      type_tparam_count[item_name] = doc_tparam_count
      for (i = 1; i <= doc_tparam_count; i++) {
        name = doc_tparam_order[i]
        type_tparam_name[item_name, i] = name
        type_tparam_text[item_name, i] = doc_tparam_text[name]
      }
      next
    }

    if (prefix != "M" && prefix != "P" && prefix != "F" && prefix != "E") next

    decl_type = get_declaring_type(item_name)
    if (length(decl_type) == 0) next

    split_type(decl_type)
    ns = split_ns
    if (length(ns) == 0) ns = "(global)"
    if (is_excluded_namespace(ns)) next

    if (!(decl_type in type_to_ns)) {
      add_namespace(ns)
      add_type(ns, decl_type, split_type_short, "type", "")
      type_to_ns[decl_type] = ns
    }

    simple_name = get_member_simple_name(item_name)
    param_sig = get_param_sig(item_name)

    if (simple_name ~ /^</) next

    if (prefix == "F") {
      if (simple_name !~ /^[A-Z]/) next
      category = "fields"
      signature = simple_name
    } else if (prefix == "P") {
      if (simple_name !~ /^[A-Z]/) next
      category = "properties"
      signature = simple_name param_sig
    } else if (prefix == "M") {
      if (simple_name == "#ctor") {
        category = "constructors"
        split_type(decl_type)
        signature = split_type_short param_sig
      } else {
        if (simple_name !~ /^[A-Z]/) next
        category = "methods"
        signature = simple_name param_sig
      }
    } else {
      category = "others"
      signature = simple_name
    }

    add_member(decl_type, category, signature, doc_summary, doc_returns)
  }

  END {
    print "# DbExport.Api Overview" > out_file
    print "" >> out_file
    print "A quick navigable guide for the DbExport API module." >> out_file
    print "" >> out_file

    print "## Index" >> out_file
    for (nsi = 1; nsi <= namespace_count; nsi++) {
      ns = namespace_order[nsi]
      print "- [Namespace `" ns "`](#" anchor("namespace " ns) ")" >> out_file
      for (ti = 1; ti <= type_count; ti++) {
        tkey = type_order[ti]
        if (type_ns[tkey] != ns) continue
        tlabel = display_type_name(type_short_name[tkey])
        print "  - [`" tlabel "`](#" anchor("type " type_full_name[tkey]) ")" >> out_file
      }
    }
    print "" >> out_file

    for (nsi = 1; nsi <= namespace_count; nsi++) {
      ns = namespace_order[nsi]
      print "<a id=\"" anchor("namespace " ns) "\"></a>" >> out_file
      print "## Namespace `" ns "`" >> out_file
      print "" >> out_file

      has_types = 0
      for (ti = 1; ti <= type_count; ti++) {
        tkey = type_order[ti]
        if (type_ns[tkey] != ns) continue
        has_types = 1

        kind = type_kind[tkey]
        print "<a id=\"" anchor("type " type_full_name[tkey]) "\"></a>" >> out_file
        print "### Type `" display_type_name(type_short_name[tkey]) "` (" kind ")" >> out_file
        print "" >> out_file

        tsum = type_summary[tkey]
        if (length(tsum) > 0) {
          print tsum >> out_file
        } else {
          print "No XML summary provided." >> out_file
        }
        print "" >> out_file

        tcount = type_tparam_count[type_full_name[tkey]]
        if (tcount > 0) {
          print "Type Parameters:" >> out_file
          for (j = 1; j <= tcount; j++) {
            pname = type_tparam_name[type_full_name[tkey], j]
            ptext = type_tparam_text[type_full_name[tkey], j]
            if (length(ptext) == 0) ptext = "No XML description provided."
            print "- `" pname "`: " ptext >> out_file
          }
          print "" >> out_file
        }

        for (ci = 1; ci <= 5; ci++) {
          cat = category_order_name(ci)
          key = type_full_name[tkey] SUBSEP cat
          if (!(key in member_list)) continue

          print "#### " category_title(cat) >> out_file
          print "" >> out_file

          split(member_list[key], mids, ",")
          for (mi = 1; mi <= length(mids); mi++) {
            mid = mids[mi] + 0
            print "- `" member_signature[mid] "`" >> out_file

            msum = member_summary[mid]
            if (length(msum) > 0) {
              print "" >> out_file
              print "  " msum >> out_file
            } else {
              print "" >> out_file
              print "  No XML summary provided." >> out_file
            }

            mtc = member_tparam_count[mid]
            if (mtc > 0) {
              print "" >> out_file
              print "  Type Parameters:" >> out_file
              for (j = 1; j <= mtc; j++) {
                tpname = member_tparam_name[mid, j]
                tptext = member_tparam_text[mid, j]
                if (length(tptext) == 0) tptext = "No XML description provided."
                print "  - `" tpname "`: " tptext >> out_file
              }
            }

            mpc = member_param_count[mid]
            if (mpc > 0) {
              print "" >> out_file
              print "  Parameters:" >> out_file
              for (j = 1; j <= mpc; j++) {
                mpname = member_param_name[mid, j]
                mptext = member_param_text[mid, j]
                if (length(mptext) == 0) mptext = "No XML description provided."
                print "  - `" mpname "`: " mptext >> out_file
              }
            }

            mret = member_returns[mid]
            if (length(mret) > 0) {
              print "" >> out_file
              print "  Returns: " mret >> out_file
            }

            mec = member_exception_count[mid]
            if (mec > 0) {
              print "" >> out_file
              print "  Throws:" >> out_file
              for (j = 1; j <= mec; j++) {
                exname = member_exception_name[mid, j]
                extext = member_exception_text[mid, j]
                if (length(extext) == 0) extext = "No XML description provided."
                print "  - `" exname "`: " extext >> out_file
              }
            }

            print "" >> out_file
          }
        }
      }

      if (!has_types) {
        print "No public types documented." >> out_file
        print "" >> out_file
      }
    }
  }
' "$XML_FILE"

printf 'Generated %s\n' "$OUT_FILE"
