<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class VB22
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim ListViewItem1 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem("Testing")
        Me.MyMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.Item01ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Item02ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MyList = New System.Windows.Forms.ListView()
        Me.Title = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.MyMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'MyMenu
        '
        Me.MyMenu.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.MyMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.Item01ToolStripMenuItem, Me.Item02ToolStripMenuItem})
        Me.MyMenu.Name = "MyMenu"
        Me.MyMenu.Size = New System.Drawing.Size(129, 52)
        '
        'Item01ToolStripMenuItem
        '
        Me.Item01ToolStripMenuItem.Name = "Item01ToolStripMenuItem"
        Me.Item01ToolStripMenuItem.Size = New System.Drawing.Size(128, 24)
        Me.Item01ToolStripMenuItem.Text = "Item 01"
        '
        'Item02ToolStripMenuItem
        '
        Me.Item02ToolStripMenuItem.Name = "Item02ToolStripMenuItem"
        Me.Item02ToolStripMenuItem.Size = New System.Drawing.Size(128, 24)
        Me.Item02ToolStripMenuItem.Text = "Item 02"
        '
        'MyList
        '
        Me.MyList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.Title})
        Me.MyList.ContextMenuStrip = Me.MyMenu
        Me.MyList.HideSelection = False
        Me.MyList.Items.AddRange(New System.Windows.Forms.ListViewItem() {ListViewItem1})
        Me.MyList.Location = New System.Drawing.Point(473, 12)
        Me.MyList.Name = "MyList"
        Me.MyList.Size = New System.Drawing.Size(496, 426)
        Me.MyList.TabIndex = 1
        Me.MyList.UseCompatibleStateImageBehavior = False
        '
        'VB22
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(981, 450)
        Me.Controls.Add(Me.MyList)
        Me.Name = "VB22"
        Me.Text = "Playground"
        Me.MyMenu.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents MyMenu As ContextMenuStrip
    Friend WithEvents Item01ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Item02ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MyList As ListView
    Friend WithEvents Title As ColumnHeader
End Class
