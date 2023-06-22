<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MyFormApp
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Description = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Title = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Procedure = New System.Windows.Forms.TextBox()
        Me.Ingredients = New System.Windows.Forms.TextBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.Status = New System.Windows.Forms.Label()
        Me.EditRecipe = New System.Windows.Forms.Button()
        Me.DeleteRecipe = New System.Windows.Forms.Button()
        Me.TheList = New System.Windows.Forms.ListView()
        Me.TitlePlace = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.DescriptionPlace = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.AddRecipe = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Description)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.Title)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(386, 197)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Main Information"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(19, 55)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(75, 16)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Description"
        '
        'Description
        '
        Me.Description.Location = New System.Drawing.Point(106, 49)
        Me.Description.Multiline = True
        Me.Description.Name = "Description"
        Me.Description.Size = New System.Drawing.Size(260, 130)
        Me.Description.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(19, 27)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(33, 16)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Title"
        '
        'Title
        '
        Me.Title.Location = New System.Drawing.Point(106, 21)
        Me.Title.Name = "Title"
        Me.Title.Size = New System.Drawing.Size(260, 22)
        Me.Title.TabIndex = 0
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.Procedure)
        Me.GroupBox2.Controls.Add(Me.Ingredients)
        Me.GroupBox2.Location = New System.Drawing.Point(12, 215)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(386, 302)
        Me.GroupBox2.TabIndex = 4
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Details"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(19, 162)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(70, 16)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Procedure"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(19, 29)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(73, 16)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Ingredients"
        '
        'Procedure
        '
        Me.Procedure.Location = New System.Drawing.Point(22, 181)
        Me.Procedure.Multiline = True
        Me.Procedure.Name = "Procedure"
        Me.Procedure.Size = New System.Drawing.Size(344, 103)
        Me.Procedure.TabIndex = 3
        '
        'Ingredients
        '
        Me.Ingredients.Location = New System.Drawing.Point(22, 48)
        Me.Ingredients.Multiline = True
        Me.Ingredients.Name = "Ingredients"
        Me.Ingredients.Size = New System.Drawing.Size(344, 103)
        Me.Ingredients.TabIndex = 2
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.Status)
        Me.GroupBox4.Controls.Add(Me.EditRecipe)
        Me.GroupBox4.Controls.Add(Me.DeleteRecipe)
        Me.GroupBox4.Controls.Add(Me.TheList)
        Me.GroupBox4.Controls.Add(Me.AddRecipe)
        Me.GroupBox4.Location = New System.Drawing.Point(413, 12)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(479, 505)
        Me.GroupBox4.TabIndex = 5
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "List of Recipies"
        '
        'Status
        '
        Me.Status.AutoSize = True
        Me.Status.Location = New System.Drawing.Point(18, 361)
        Me.Status.Name = "Status"
        Me.Status.Size = New System.Drawing.Size(0, 16)
        Me.Status.TabIndex = 9
        '
        'EditRecipe
        '
        Me.EditRecipe.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!)
        Me.EditRecipe.ForeColor = System.Drawing.SystemColors.HotTrack
        Me.EditRecipe.Location = New System.Drawing.Point(168, 384)
        Me.EditRecipe.Name = "EditRecipe"
        Me.EditRecipe.Size = New System.Drawing.Size(145, 103)
        Me.EditRecipe.TabIndex = 5
        Me.EditRecipe.Text = "Update"
        Me.EditRecipe.UseVisualStyleBackColor = True
        '
        'DeleteRecipe
        '
        Me.DeleteRecipe.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!)
        Me.DeleteRecipe.ForeColor = System.Drawing.Color.Crimson
        Me.DeleteRecipe.Location = New System.Drawing.Point(319, 384)
        Me.DeleteRecipe.Name = "DeleteRecipe"
        Me.DeleteRecipe.Size = New System.Drawing.Size(139, 103)
        Me.DeleteRecipe.TabIndex = 6
        Me.DeleteRecipe.Text = "Delete"
        Me.DeleteRecipe.UseVisualStyleBackColor = True
        '
        'TheList
        '
        Me.TheList.AllowColumnReorder = True
        Me.TheList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.TitlePlace, Me.DescriptionPlace})
        Me.TheList.FullRowSelect = True
        Me.TheList.HideSelection = False
        Me.TheList.Location = New System.Drawing.Point(17, 48)
        Me.TheList.MultiSelect = False
        Me.TheList.Name = "TheList"
        Me.TheList.Size = New System.Drawing.Size(441, 306)
        Me.TheList.TabIndex = 8
        Me.TheList.UseCompatibleStateImageBehavior = False
        Me.TheList.View = System.Windows.Forms.View.Details
        '
        'TitlePlace
        '
        Me.TitlePlace.Text = "Titles"
        Me.TitlePlace.Width = 129
        '
        'DescriptionPlace
        '
        Me.DescriptionPlace.Text = "Descriptions"
        Me.DescriptionPlace.Width = 212
        '
        'AddRecipe
        '
        Me.AddRecipe.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AddRecipe.ForeColor = System.Drawing.Color.Green
        Me.AddRecipe.Location = New System.Drawing.Point(17, 384)
        Me.AddRecipe.Name = "AddRecipe"
        Me.AddRecipe.Size = New System.Drawing.Size(145, 103)
        Me.AddRecipe.TabIndex = 4
        Me.AddRecipe.Text = "Add"
        Me.AddRecipe.UseVisualStyleBackColor = True
        '
        'MyFormApp
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(904, 529)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "MyFormApp"
        Me.Text = "MyFormApp"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox4.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Title As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Description As TextBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Procedure As TextBox
    Friend WithEvents Ingredients As TextBox
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents AddRecipe As Button
    Friend WithEvents TheList As ListView
    Friend WithEvents TitlePlace As ColumnHeader
    Friend WithEvents EditRecipe As Button
    Friend WithEvents DeleteRecipe As Button
    Friend WithEvents DescriptionPlace As ColumnHeader
    Friend WithEvents Status As Label
End Class
