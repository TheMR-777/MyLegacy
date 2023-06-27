<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ReportApp
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ReportApp))
        Me.G_DescriptionBox = New System.Windows.Forms.GroupBox()
        Me.MyDivider = New System.Windows.Forms.Label()
        Me.Title = New System.Windows.Forms.Label()
        Me.Description = New System.Windows.Forms.Label()
        Me.G_ImageBox = New System.Windows.Forms.GroupBox()
        Me.ImageBox_Label = New System.Windows.Forms.Label()
        Me.ImageBox = New System.Windows.Forms.PictureBox()
        Me.G_IngredientsBox = New System.Windows.Forms.GroupBox()
        Me.Ingredients = New System.Windows.Forms.Label()
        Me.G_ProcedureBox = New System.Windows.Forms.GroupBox()
        Me.Procedure = New System.Windows.Forms.Label()
        Me.G_DescriptionBox.SuspendLayout()
        Me.G_ImageBox.SuspendLayout()
        CType(Me.ImageBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.G_IngredientsBox.SuspendLayout()
        Me.G_ProcedureBox.SuspendLayout()
        Me.SuspendLayout()
        '
        'G_DescriptionBox
        '
        Me.G_DescriptionBox.Controls.Add(Me.MyDivider)
        Me.G_DescriptionBox.Controls.Add(Me.Title)
        Me.G_DescriptionBox.Controls.Add(Me.Description)
        Me.G_DescriptionBox.Location = New System.Drawing.Point(12, 12)
        Me.G_DescriptionBox.Name = "G_DescriptionBox"
        Me.G_DescriptionBox.Size = New System.Drawing.Size(402, 350)
        Me.G_DescriptionBox.TabIndex = 0
        Me.G_DescriptionBox.TabStop = False
        Me.G_DescriptionBox.Text = "Recipe"
        '
        'MyDivider
        '
        Me.MyDivider.AutoSize = True
        Me.MyDivider.ForeColor = System.Drawing.SystemColors.ControlLight
        Me.MyDivider.Location = New System.Drawing.Point(21, 70)
        Me.MyDivider.Name = "MyDivider"
        Me.MyDivider.Size = New System.Drawing.Size(358, 16)
        Me.MyDivider.TabIndex = 12
        Me.MyDivider.Text = "———————————————————————————————————————"
        '
        'Title
        '
        Me.Title.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!)
        Me.Title.Location = New System.Drawing.Point(21, 18)
        Me.Title.Name = "Title"
        Me.Title.Size = New System.Drawing.Size(359, 52)
        Me.Title.TabIndex = 1
        Me.Title.Text = "The Title Text"
        Me.Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Description
        '
        Me.Description.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.8!, System.Drawing.FontStyle.Italic)
        Me.Description.Location = New System.Drawing.Point(21, 99)
        Me.Description.Name = "Description"
        Me.Description.Size = New System.Drawing.Size(359, 228)
        Me.Description.TabIndex = 0
        Me.Description.Text = "The Description"
        Me.Description.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'G_ImageBox
        '
        Me.G_ImageBox.Controls.Add(Me.ImageBox_Label)
        Me.G_ImageBox.Controls.Add(Me.ImageBox)
        Me.G_ImageBox.Location = New System.Drawing.Point(420, 12)
        Me.G_ImageBox.Name = "G_ImageBox"
        Me.G_ImageBox.Size = New System.Drawing.Size(350, 350)
        Me.G_ImageBox.TabIndex = 1
        Me.G_ImageBox.TabStop = False
        Me.G_ImageBox.Text = "Display Picture"
        '
        'ImageBox_Label
        '
        Me.ImageBox_Label.Location = New System.Drawing.Point(21, 34)
        Me.ImageBox_Label.Name = "ImageBox_Label"
        Me.ImageBox_Label.Size = New System.Drawing.Size(306, 293)
        Me.ImageBox_Label.TabIndex = 1
        Me.ImageBox_Label.Text = "No Image Available"
        Me.ImageBox_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ImageBox
        '
        Me.ImageBox.Location = New System.Drawing.Point(21, 34)
        Me.ImageBox.Name = "ImageBox"
        Me.ImageBox.Size = New System.Drawing.Size(306, 293)
        Me.ImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.ImageBox.TabIndex = 0
        Me.ImageBox.TabStop = False
        '
        'G_IngredientsBox
        '
        Me.G_IngredientsBox.Controls.Add(Me.Ingredients)
        Me.G_IngredientsBox.Location = New System.Drawing.Point(12, 368)
        Me.G_IngredientsBox.Name = "G_IngredientsBox"
        Me.G_IngredientsBox.Size = New System.Drawing.Size(171, 273)
        Me.G_IngredientsBox.TabIndex = 1
        Me.G_IngredientsBox.TabStop = False
        Me.G_IngredientsBox.Text = "Ingredients List"
        '
        'Ingredients
        '
        Me.Ingredients.Location = New System.Drawing.Point(6, 28)
        Me.Ingredients.Name = "Ingredients"
        Me.Ingredients.Padding = New System.Windows.Forms.Padding(7, 0, 0, 0)
        Me.Ingredients.Size = New System.Drawing.Size(159, 230)
        Me.Ingredients.TabIndex = 13
        Me.Ingredients.Text = "The Ingredients"
        Me.Ingredients.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'G_ProcedureBox
        '
        Me.G_ProcedureBox.Controls.Add(Me.Procedure)
        Me.G_ProcedureBox.Location = New System.Drawing.Point(189, 368)
        Me.G_ProcedureBox.Name = "G_ProcedureBox"
        Me.G_ProcedureBox.Size = New System.Drawing.Size(581, 273)
        Me.G_ProcedureBox.TabIndex = 2
        Me.G_ProcedureBox.TabStop = False
        Me.G_ProcedureBox.Text = "Procedure (Method)"
        '
        'Procedure
        '
        Me.Procedure.Location = New System.Drawing.Point(20, 28)
        Me.Procedure.Name = "Procedure"
        Me.Procedure.Size = New System.Drawing.Size(538, 228)
        Me.Procedure.TabIndex = 13
        Me.Procedure.Text = "The Procedure"
        Me.Procedure.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ReportApp
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(782, 653)
        Me.Controls.Add(Me.G_ProcedureBox)
        Me.Controls.Add(Me.G_IngredientsBox)
        Me.Controls.Add(Me.G_ImageBox)
        Me.Controls.Add(Me.G_DescriptionBox)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "ReportApp"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Report"
        Me.G_DescriptionBox.ResumeLayout(False)
        Me.G_DescriptionBox.PerformLayout()
        Me.G_ImageBox.ResumeLayout(False)
        CType(Me.ImageBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.G_IngredientsBox.ResumeLayout(False)
        Me.G_ProcedureBox.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents G_DescriptionBox As GroupBox
    Friend WithEvents G_ImageBox As GroupBox
    Friend WithEvents G_IngredientsBox As GroupBox
    Friend WithEvents G_ProcedureBox As GroupBox
    Friend WithEvents Description As Label
    Friend WithEvents ImageBox As PictureBox
    Friend WithEvents Title As Label
    Friend WithEvents MyDivider As Label
    Friend WithEvents Ingredients As Label
    Friend WithEvents Procedure As Label
    Friend WithEvents ImageBox_Label As Label
End Class
