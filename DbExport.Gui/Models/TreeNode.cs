using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using DbExport.Schema;

namespace DbExport.Gui.Models;

public enum TreeNodeType
{
    Schema,
    Group,
    Table,
    Column,
    PrimaryKey,
    ForeignKey,
    Index,
    DataType
}

public partial class TreeNode : ObservableObject
{
    private static bool cascadingCheckState;
    private readonly ICheckable? checkable;
    
    [ObservableProperty]
    private bool? isChecked = false;
    
    [ObservableProperty]
    private bool isExpanded;

    public TreeNode(TreeNode? parent, string text, TreeNodeType type)
    {
        Parent = parent;
        Text = text;
        Type = type;
    }

    public TreeNode(TreeNode? parent, Table table) : this(parent, table.Name, TreeNodeType.Table)
    {
        checkable = table;
        
        var columnsNode = new TreeNode(this, "Columns", TreeNodeType.Group);
        columnsNode.Children.AddRange(table.Columns.Select(c => new TreeNode(columnsNode, c)));
        Children.Add(columnsNode);
        
        var indexesNode = new TreeNode(this, "Indexes", TreeNodeType.Group);
        indexesNode.Children.AddRange(table.Indexes.Where(i => !i.MatchesKey)
                                           .Select(i => new TreeNode(indexesNode, i)));
        Children.Add(indexesNode);
        
        var foreignKeysNode = new TreeNode(this, "Foreign keys", TreeNodeType.Group);
        foreignKeysNode.Children.AddRange(table.ForeignKeys.Select(fk => new TreeNode(foreignKeysNode, fk)));
        Children.Add(foreignKeysNode);
    }

    public TreeNode(TreeNode? parent, Column column) :
        this(parent, column.Name, column.IsPKColumn ? TreeNodeType.PrimaryKey : TreeNodeType.Column)
    {
        checkable = column;
    }

    public TreeNode(TreeNode? parent, Index index) : this(parent, index.Name, TreeNodeType.Index)
    {
        checkable = index;
    }

    public TreeNode(TreeNode? parent, ForeignKey fk) :
        this(parent, $"{fk.Name} \u2192 {fk.RelatedTableFullName}", TreeNodeType.ForeignKey)
    {
        checkable = fk;
    }

    public TreeNode(TreeNode? parent, DataType dataType) :
        this(parent, dataType.Name, TreeNodeType.DataType)
    {
        checkable = dataType;
    }
    
    public TreeNode? Parent { get; }
    
    public string Text { get; }
    
    public TreeNodeType Type { get; }

    public string Icon => Type switch
    {
        TreeNodeType.Schema => "fa-database",
        TreeNodeType.Group => "fa-folder",
        TreeNodeType.Table => "fa-table",
        TreeNodeType.Column => "fa-table-columns",
        TreeNodeType.PrimaryKey => "fa-key",
        TreeNodeType.ForeignKey => "fa-link",
        TreeNodeType.Index => "fa-sort",
        TreeNodeType.DataType => "fa-shapes",
        _ => string.Empty
    };
    
    public List<TreeNode> Children { get; } = [];
    
    public static IEnumerable<TreeNode> FromDatabase(Database database)
    {
        var schemas = database.Tables.GroupBy(t => t.Owner);

        foreach (var schema in schemas)
        {
            var schemaName = string.IsNullOrEmpty(schema.Key) ? "Default schema" : schema.Key;
            var schemaNode = new TreeNode(null, schemaName, TreeNodeType.Schema);
            
            var tablesNode = new TreeNode(schemaNode, "Tables", TreeNodeType.Group);
            tablesNode.Children.AddRange(schema.Select(t => new TreeNode(tablesNode, t)));
            schemaNode.Children.Add(tablesNode);
            
            var dataTypesNode = new TreeNode(schemaNode, "Data types", TreeNodeType.Group);
            var schemaDataTypes = database.DataTypes.Where(t => t.Owner == schema.Key);
            dataTypesNode.Children.AddRange(schemaDataTypes.Select(t => new TreeNode(dataTypesNode, t)));
            schemaNode.Children.Add(dataTypesNode);
            
            schemaNode.IsChecked = schemaNode.IsExpanded = true;
            
            yield return schemaNode;
        }
    }

    partial void OnIsCheckedChanged(bool? value)
    {
        if (value == null) return;
        checkable?.IsChecked = value.Value;
        
        if (cascadingCheckState) return;
        
        cascadingCheckState = true;
        CascadeCheckStateForward(value.Value);
        CascadeCheckStateBackward(Parent);
        cascadingCheckState = false;
    }

    private void CascadeCheckStateForward(bool checkState)
    {
        foreach (var child in Children)
        {
            child.IsChecked = checkState;
            child.CascadeCheckStateForward(checkState);
        }
    }

    private static void CascadeCheckStateBackward(TreeNode? node)
    {
        while (node != null)
        {
            var allChecked = node.Children.All(c => c.IsChecked.HasValue && c.IsChecked.Value);
            var allUnchecked = node.Children.All(c => c.IsChecked.HasValue && !c.IsChecked.Value);

            if (allChecked)
                node.IsChecked = true;
            else if (allUnchecked)
                node.IsChecked = false;
            else
                node.IsChecked = null;

            node = node.Parent;
        }
    }
}