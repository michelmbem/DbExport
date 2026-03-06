#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
API_DIR="$ROOT_DIR/DbExport.Api"
OUT_DIR="$ROOT_DIR/docs"
OUT_FILE="$OUT_DIR/DbExport.Api.md"

mkdir -p "$OUT_DIR"

{
  printf '# DbExport.Api Documentation\n\n'
  printf 'Auto-generated from XML doc comments in the DbExport.Api module.\n\n'
  printf '## Index\n'
  find "$API_DIR" -type f -name '*.cs' ! -path '*/obj/*' | sort | \
    sed "s#^$API_DIR/##; s#\.cs\$##" | \
    awk '{printf "- [%s](#%s)\n", $0, tolower($0)}'
  printf '\n'

  find "$API_DIR" -type f -name '*.cs' ! -path '*/obj/*' | sort | while read -r f; do
    rel="${f#"$API_DIR"/}"
    sec="${rel%.cs}"
    printf '## %s\n\n' "$sec"

    awk '
      function normalize_ref(ref) {
        sub(/^.*:/, "", ref)
        gsub("{", "<", ref)
        gsub("}", ">", ref)
        return ref
      }

      BEGIN {
        in_summary=0
        pending_summary=0
        summary=""
      }

      /^[[:space:]]*\/\/\/ <summary>/ {
        in_summary=1
        pending_summary=0
        summary=""
        next
      }

      in_summary {
        if ($0 ~ /<\/summary>/) {
          in_summary=0
          pending_summary=1
          next
        }

        line=$0
        sub(/^[[:space:]]*\/\/\/ ?/, "", line)

        while (match(line, /<see[[:space:]]+cref="[^"]+"[[:space:]]*\/>/)) {
          tag=substr(line, RSTART, RLENGTH)
          ref=tag
          sub(/^.*cref="/, "", ref)
          sub(/"[[:space:]]*\/>$/, "", ref)
          ref=normalize_ref(ref)
          line=substr(line, 1, RSTART-1) ref substr(line, RSTART+RLENGTH)
        }

        while (match(line, /<see[[:space:]]+langword="[^"]+"[[:space:]]*\/>/)) {
          tag=substr(line, RSTART, RLENGTH)
          word=tag
          sub(/^.*langword="/, "", word)
          sub(/"[[:space:]]*\/>$/, "", word)
          line=substr(line, 1, RSTART-1) word substr(line, RSTART+RLENGTH)
        }

        gsub(/<[^>]+>/, "", line)
        gsub(/^[[:space:]]+|[[:space:]]+$/, "", line)
        if (length(line) > 0) {
          if (length(summary) > 0) summary = summary " "
          summary = summary line
        }
        next
      }

      pending_summary {
        if ($0 ~ /^[[:space:]]*\/\/\//) next
        if ($0 ~ /^[[:space:]]*\[/) next
        if ($0 ~ /^[[:space:]]*$/) next
        if ($0 ~ /^[[:space:]]*#(region|endregion|if|endif|pragma)/) next

        sig=$0
        gsub(/^[[:space:]]+/, "", sig)

        if (sig ~ /^(public)[[:space:]]/) {
          print "- `" sig "`"
          print ""
          print "  " summary
          print ""
        }

        pending_summary=0
        summary=""
        next
      }

      {
        sig=$0
        gsub(/^[[:space:]]+/, "", sig)
        if (sig ~ /^(public)[[:space:]]/) {
          print "- `" sig "`"
          print ""
          print "  No XML summary provided."
          print ""
        }
      }
    ' "$f"

    printf '\n'
  done
} > "$OUT_FILE"

printf 'Generated %s\n' "$OUT_FILE"
