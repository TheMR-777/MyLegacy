<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class RecipeManager
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
        Me.Clear = New System.Windows.Forms.Button()
        Me.Status = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Description = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Title = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Procedure = New System.Windows.Forms.TextBox()
        Me.Ingredients = New System.Windows.Forms.TextBox()
        Me.AddRecipe = New System.Windows.Forms.Button()
        Me.DeleteRecipe = New System.Windows.Forms.Button()
        Me.TheList = New System.Windows.Forms.ListView()
        Me.TitlePlace = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.DescriptionPlace = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.Locale = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Clear)
        Me.GroupBox1.Controls.Add(Me.Status)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Description)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.Title)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(386, 167)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Main Information"
        '
        'Clear
        '
        Me.Clear.Location = New System.Drawing.Point(19, 84)
        Me.Clear.Name = "Clear"
        Me.Clear.Size = New System.Drawing.Size(75, 36)
        Me.Clear.TabIndex = 10
        Me.Clear.Text = "Clear All"
        Me.Clear.UseVisualStyleBackColor = True
        '
        'Status
        '
        Me.Status.AutoSize = True
        Me.Status.Location = New System.Drawing.Point(19, 132)
        Me.Status.Name = "Status"
        Me.Status.Size = New System.Drawing.Size(62, 16)
        Me.Status.TabIndex = 9
        Me.Status.Text = "MyStatus"
        Me.Status.TextAlign = System.Drawing.ContentAlignment.MiddleRight
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
        Me.Description.Size = New System.Drawing.Size(260, 99)
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
        Me.GroupBox2.Location = New System.Drawing.Point(12, 185)
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
        'AddRecipe
        '
        Me.AddRecipe.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AddRecipe.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.AddRecipe.Location = New System.Drawing.Point(12, 502)
        Me.AddRecipe.Name = "AddRecipe"
        Me.AddRecipe.Size = New System.Drawing.Size(184, 59)
        Me.AddRecipe.TabIndex = 4
        Me.AddRecipe.Text = "Add"
        Me.AddRecipe.UseVisualStyleBackColor = True
        '
        'DeleteRecipe
        '
        Me.DeleteRecipe.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DeleteRecipe.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.DeleteRecipe.Location = New System.Drawing.Point(202, 502)
        Me.DeleteRecipe.Name = "DeleteRecipe"
        Me.DeleteRecipe.Size = New System.Drawing.Size(196, 59)
        Me.DeleteRecipe.TabIndex = 5
        Me.DeleteRecipe.Text = "Delete"
        Me.DeleteRecipe.UseVisualStyleBackColor = True
        '
        'TheList
        '
        Me.TheList.AllowColumnReorder = True
        Me.TheList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.TitlePlace, Me.DescriptionPlace})
        Me.TheList.FullRowSelect = True
        Me.TheList.HideSelection = False
        Me.TheList.Location = New System.Drawing.Point(17, 27)
        Me.TheList.MultiSelect = False
        Me.TheList.Name = "TheList"
        Me.TheList.Size = New System.Drawing.Size(392, 430)
        Me.TheList.TabIndex = 8
        Me.TheList.UseCompatibleStateImageBehavior = False
        Me.TheList.View = System.Windows.Forms.View.Details
        '
        'TitlePlace
        '
        Me.TitlePlace.Text = "Title"
        Me.TitlePlace.Width = 129
        '
        'DescriptionPlace
        '
        Me.DescriptionPlace.Text = "Description"
        Me.DescriptionPlace.Width = 212
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.TheList)
        Me.GroupBox4.Location = New System.Drawing.Point(413, 12)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(427, 475)
        Me.GroupBox4.TabIndex = 5
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "List of Recipies"
        '
        'Locale
        '
        Me.Locale.AutoSize = True
        Me.Locale.Location = New System.Drawing.Point(14, 31)
        Me.Locale.Name = "Locale"
        Me.Locale.Size = New System.Drawing.Size(237, 16)
        Me.Locale.TabIndex = 6
        Me.Locale.Text = "IP: 192.168.0.100  |  Timezone: Pakistan"
        Me.Locale.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Locale)
        Me.GroupBox3.Location = New System.Drawing.Point(413, 497)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(427, 64)
        Me.GroupBox3.TabIndex = 7
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Locale Information"
        '
        'RecipeManager
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(852, 573)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.DeleteRecipe)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.AddRecipe)
        Me.Name = "RecipeManager"
        Me.Text = "Recipe Manager"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
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
    Friend WithEvents AddRecipe As Button
    Friend WithEvents DeleteRecipe As Button
    Friend WithEvents TheList As ListView
    Friend WithEvents TitlePlace As ColumnHeader
    Friend WithEvents DescriptionPlace As ColumnHeader
    Friend WithEvents Status As Label
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents Clear As Button
    Friend WithEvents Locale As Label
    Friend WithEvents GroupBox3 As GroupBox
End Class
