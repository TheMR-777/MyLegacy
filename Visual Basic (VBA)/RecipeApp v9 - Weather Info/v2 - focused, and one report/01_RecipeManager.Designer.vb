<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class RecipeApp
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RecipeApp))
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.MyDivider = New System.Windows.Forms.Label()
        Me.Clear = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Description = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Title = New System.Windows.Forms.TextBox()
        Me.Status = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Procedure = New System.Windows.Forms.TextBox()
        Me.Ingredients = New System.Windows.Forms.TextBox()
        Me.AddRecipe = New System.Windows.Forms.Button()
        Me.DeleteRecipe = New System.Windows.Forms.Button()
        Me.TheRecipeList = New System.Windows.Forms.ListView()
        Me.TitlePlace = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.DescriptionPlace = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ListMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ViewReportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeleteItemToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.Locale = New System.Windows.Forms.Label()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.ImageBox = New System.Windows.Forms.GroupBox()
        Me.ImageDisplay_Label = New System.Windows.Forms.Label()
        Me.ImageDisplay = New System.Windows.Forms.PictureBox()
        Me.TopMenu = New System.Windows.Forms.MenuStrip()
        Me.RDLCReportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewReportRDLC = New System.Windows.Forms.ToolStripMenuItem()
        Me.SignOutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.WeatherDescription = New System.Windows.Forms.Label()
        Me.Welcome = New System.Windows.Forms.Label()
        Me.Weather = New System.Windows.Forms.Label()
        Me.WeatherTemperature = New System.Windows.Forms.Label()
        Me.WeatherLogo = New System.Windows.Forms.PictureBox()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.ListMenu.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.ImageBox.SuspendLayout()
        CType(Me.ImageDisplay, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TopMenu.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        CType(Me.WeatherLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.MyDivider)
        Me.GroupBox1.Controls.Add(Me.Clear)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Description)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.Title)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 33)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(386, 167)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Main Information"
        '
        'MyDivider
        '
        Me.MyDivider.AutoSize = True
        Me.MyDivider.ForeColor = System.Drawing.SystemColors.ControlLight
        Me.MyDivider.Location = New System.Drawing.Point(18, 78)
        Me.MyDivider.Name = "MyDivider"
        Me.MyDivider.Size = New System.Drawing.Size(79, 16)
        Me.MyDivider.TabIndex = 11
        Me.MyDivider.Text = "————————"
        '
        'Clear
        '
        Me.Clear.Location = New System.Drawing.Point(19, 104)
        Me.Clear.Name = "Clear"
        Me.Clear.Size = New System.Drawing.Size(75, 44)
        Me.Clear.TabIndex = 10
        Me.Clear.Text = "Clear All"
        Me.Clear.UseVisualStyleBackColor = True
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
        'Status
        '
        Me.Status.BackColor = System.Drawing.SystemColors.ButtonHighlight
        Me.Status.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!)
        Me.Status.Location = New System.Drawing.Point(777, 6)
        Me.Status.Name = "Status"
        Me.Status.Size = New System.Drawing.Size(75, 16)
        Me.Status.TabIndex = 9
        Me.Status.Text = "MyStatus"
        Me.Status.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Controls.Add(Me.Procedure)
        Me.GroupBox2.Controls.Add(Me.Ingredients)
        Me.GroupBox2.Location = New System.Drawing.Point(12, 365)
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
        Me.AddRecipe.Location = New System.Drawing.Point(12, 682)
        Me.AddRecipe.Name = "AddRecipe"
        Me.AddRecipe.Size = New System.Drawing.Size(184, 59)
        Me.AddRecipe.TabIndex = 4
        Me.AddRecipe.Text = "The Creator"
        Me.AddRecipe.UseVisualStyleBackColor = True
        '
        'DeleteRecipe
        '
        Me.DeleteRecipe.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DeleteRecipe.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.DeleteRecipe.Location = New System.Drawing.Point(202, 682)
        Me.DeleteRecipe.Name = "DeleteRecipe"
        Me.DeleteRecipe.Size = New System.Drawing.Size(196, 59)
        Me.DeleteRecipe.TabIndex = 5
        Me.DeleteRecipe.Text = "Delete"
        Me.DeleteRecipe.UseVisualStyleBackColor = True
        '
        'TheRecipeList
        '
        Me.TheRecipeList.AllowColumnReorder = True
        Me.TheRecipeList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.TitlePlace, Me.DescriptionPlace})
        Me.TheRecipeList.ContextMenuStrip = Me.ListMenu
        Me.TheRecipeList.FullRowSelect = True
        Me.TheRecipeList.HideSelection = False
        Me.TheRecipeList.Location = New System.Drawing.Point(17, 30)
        Me.TheRecipeList.MultiSelect = False
        Me.TheRecipeList.Name = "TheRecipeList"
        Me.TheRecipeList.Size = New System.Drawing.Size(422, 413)
        Me.TheRecipeList.TabIndex = 8
        Me.TheRecipeList.UseCompatibleStateImageBehavior = False
        Me.TheRecipeList.View = System.Windows.Forms.View.Details
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
        'ListMenu
        '
        Me.ListMenu.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.ListMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewReportToolStripMenuItem, Me.DeleteItemToolStripMenuItem})
        Me.ListMenu.Name = "ListMenu"
        Me.ListMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        Me.ListMenu.Size = New System.Drawing.Size(160, 52)
        Me.ListMenu.Text = "Nice"
        '
        'ViewReportToolStripMenuItem
        '
        Me.ViewReportToolStripMenuItem.Name = "ViewReportToolStripMenuItem"
        Me.ViewReportToolStripMenuItem.Size = New System.Drawing.Size(159, 24)
        Me.ViewReportToolStripMenuItem.Text = "View Report"
        '
        'DeleteItemToolStripMenuItem
        '
        Me.DeleteItemToolStripMenuItem.Name = "DeleteItemToolStripMenuItem"
        Me.DeleteItemToolStripMenuItem.Size = New System.Drawing.Size(159, 24)
        Me.DeleteItemToolStripMenuItem.Text = "Delete Item"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.TheRecipeList)
        Me.GroupBox4.Location = New System.Drawing.Point(413, 206)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(457, 461)
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
        Me.GroupBox3.Location = New System.Drawing.Point(413, 677)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(457, 64)
        Me.GroupBox3.TabIndex = 7
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Locale Information"
        '
        'ImageBox
        '
        Me.ImageBox.Controls.Add(Me.ImageDisplay_Label)
        Me.ImageBox.Controls.Add(Me.ImageDisplay)
        Me.ImageBox.Location = New System.Drawing.Point(12, 206)
        Me.ImageBox.Name = "ImageBox"
        Me.ImageBox.Size = New System.Drawing.Size(386, 153)
        Me.ImageBox.TabIndex = 8
        Me.ImageBox.TabStop = False
        Me.ImageBox.Text = "Display Picture"
        '
        'ImageDisplay_Label
        '
        Me.ImageDisplay_Label.AutoSize = True
        Me.ImageDisplay_Label.Location = New System.Drawing.Point(125, 75)
        Me.ImageDisplay_Label.Name = "ImageDisplay_Label"
        Me.ImageDisplay_Label.Size = New System.Drawing.Size(137, 16)
        Me.ImageDisplay_Label.TabIndex = 1
        Me.ImageDisplay_Label.Text = "Click to Add an Image"
        '
        'ImageDisplay
        '
        Me.ImageDisplay.Location = New System.Drawing.Point(22, 30)
        Me.ImageDisplay.Name = "ImageDisplay"
        Me.ImageDisplay.Size = New System.Drawing.Size(344, 107)
        Me.ImageDisplay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.ImageDisplay.TabIndex = 0
        Me.ImageDisplay.TabStop = False
        '
        'TopMenu
        '
        Me.TopMenu.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.TopMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RDLCReportToolStripMenuItem})
        Me.TopMenu.Location = New System.Drawing.Point(0, 0)
        Me.TopMenu.Name = "TopMenu"
        Me.TopMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        Me.TopMenu.Size = New System.Drawing.Size(882, 28)
        Me.TopMenu.TabIndex = 9
        Me.TopMenu.Text = "MenuStrip1"
        '
        'RDLCReportToolStripMenuItem
        '
        Me.RDLCReportToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewReportRDLC, Me.SignOutToolStripMenuItem})
        Me.RDLCReportToolStripMenuItem.Font = New System.Drawing.Font("Segoe UI", 8.8!)
        Me.RDLCReportToolStripMenuItem.Margin = New System.Windows.Forms.Padding(5, 0, 0, 0)
        Me.RDLCReportToolStripMenuItem.Name = "RDLCReportToolStripMenuItem"
        Me.RDLCReportToolStripMenuItem.Size = New System.Drawing.Size(66, 24)
        Me.RDLCReportToolStripMenuItem.Text = "Action"
        '
        'ViewReportRDLC
        '
        Me.ViewReportRDLC.Name = "ViewReportRDLC"
        Me.ViewReportRDLC.Size = New System.Drawing.Size(224, 26)
        Me.ViewReportRDLC.Text = "View RDLC Report"
        '
        'SignOutToolStripMenuItem
        '
        Me.SignOutToolStripMenuItem.Name = "SignOutToolStripMenuItem"
        Me.SignOutToolStripMenuItem.Size = New System.Drawing.Size(224, 26)
        Me.SignOutToolStripMenuItem.Text = "Sign-Out"
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.WeatherDescription)
        Me.GroupBox5.Controls.Add(Me.Welcome)
        Me.GroupBox5.Controls.Add(Me.Weather)
        Me.GroupBox5.Controls.Add(Me.WeatherTemperature)
        Me.GroupBox5.Controls.Add(Me.WeatherLogo)
        Me.GroupBox5.Location = New System.Drawing.Point(413, 33)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(457, 167)
        Me.GroupBox5.TabIndex = 10
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Weather Information"
        '
        'WeatherDescription
        '
        Me.WeatherDescription.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.0!)
        Me.WeatherDescription.Location = New System.Drawing.Point(157, 119)
        Me.WeatherDescription.Name = "WeatherDescription"
        Me.WeatherDescription.Size = New System.Drawing.Size(226, 18)
        Me.WeatherDescription.TabIndex = 4
        Me.WeatherDescription.Text = "make sure to bring your umbrella"
        Me.WeatherDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Welcome
        '
        Me.Welcome.Location = New System.Drawing.Point(226, 18)
        Me.Welcome.Name = "Welcome"
        Me.Welcome.Size = New System.Drawing.Size(213, 23)
        Me.Welcome.TabIndex = 3
        Me.Welcome.Text = "Welcome TheMR"
        Me.Welcome.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Weather
        '
        Me.Weather.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Weather.Location = New System.Drawing.Point(153, 78)
        Me.Weather.Name = "Weather"
        Me.Weather.Size = New System.Drawing.Size(226, 41)
        Me.Weather.TabIndex = 2
        Me.Weather.Text = "Thunderstorm"
        Me.Weather.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'WeatherTemperature
        '
        Me.WeatherTemperature.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.WeatherTemperature.Location = New System.Drawing.Point(153, 39)
        Me.WeatherTemperature.Name = "WeatherTemperature"
        Me.WeatherTemperature.Size = New System.Drawing.Size(150, 32)
        Me.WeatherTemperature.TabIndex = 1
        Me.WeatherTemperature.Text = "27 C"
        Me.WeatherTemperature.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'WeatherLogo
        '
        Me.WeatherLogo.InitialImage = Nothing
        Me.WeatherLogo.Location = New System.Drawing.Point(17, 27)
        Me.WeatherLogo.MinimumSize = New System.Drawing.Size(100, 100)
        Me.WeatherLogo.Name = "WeatherLogo"
        Me.WeatherLogo.Size = New System.Drawing.Size(130, 120)
        Me.WeatherLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.WeatherLogo.TabIndex = 0
        Me.WeatherLogo.TabStop = False
        '
        'RecipeApp
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(882, 753)
        Me.Controls.Add(Me.GroupBox5)
        Me.Controls.Add(Me.Status)
        Me.Controls.Add(Me.TopMenu)
        Me.Controls.Add(Me.ImageBox)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.DeleteRecipe)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.AddRecipe)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.TopMenu
        Me.Name = "RecipeApp"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Recipe Manager"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ListMenu.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ImageBox.ResumeLayout(False)
        Me.ImageBox.PerformLayout()
        CType(Me.ImageDisplay, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TopMenu.ResumeLayout(False)
        Me.TopMenu.PerformLayout()
        Me.GroupBox5.ResumeLayout(False)
        CType(Me.WeatherLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

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
    Friend WithEvents TheRecipeList As ListView
    Friend WithEvents TitlePlace As ColumnHeader
    Friend WithEvents DescriptionPlace As ColumnHeader
    Friend WithEvents Status As Label
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents Clear As Button
    Friend WithEvents Locale As Label
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents MyDivider As Label
    Friend WithEvents ImageBox As GroupBox
    Friend WithEvents ImageDisplay As PictureBox
    Friend WithEvents ImageDisplay_Label As Label
    Friend WithEvents ListMenu As ContextMenuStrip
    Friend WithEvents ViewReportToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DeleteItemToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TopMenu As MenuStrip
    Friend WithEvents RDLCReportToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ViewReportRDLC As ToolStripMenuItem
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents Weather As Label
    Friend WithEvents WeatherTemperature As Label
    Friend WithEvents WeatherLogo As PictureBox
    Friend WithEvents WeatherDescription As Label
    Friend WithEvents Welcome As Label
    Friend WithEvents SignOutToolStripMenuItem As ToolStripMenuItem
End Class
