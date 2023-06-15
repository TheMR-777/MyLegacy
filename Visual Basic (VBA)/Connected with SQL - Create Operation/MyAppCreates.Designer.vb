<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MyApp
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.T_Name = New System.Windows.Forms.TextBox()
        Me.Label_Note = New System.Windows.Forms.Label()
        Me.Label_Roll = New System.Windows.Forms.Label()
        Me.Label_Name = New System.Windows.Forms.Label()
        Me.T_Roll = New System.Windows.Forms.TextBox()
        Me.T_Note = New System.Windows.Forms.TextBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Button1)
        Me.GroupBox1.Controls.Add(Me.T_Note)
        Me.GroupBox1.Controls.Add(Me.T_Roll)
        Me.GroupBox1.Controls.Add(Me.T_Name)
        Me.GroupBox1.Controls.Add(Me.Label_Note)
        Me.GroupBox1.Controls.Add(Me.Label_Roll)
        Me.GroupBox1.Controls.Add(Me.Label_Name)
        Me.GroupBox1.Location = New System.Drawing.Point(58, 39)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(448, 288)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Details will permanently update the Database"
        '
        'T_Name
        '
        Me.T_Name.Location = New System.Drawing.Point(112, 57)
        Me.T_Name.Name = "T_Name"
        Me.T_Name.Size = New System.Drawing.Size(289, 22)
        Me.T_Name.TabIndex = 3
        Me.T_Name.Text = "TheMR"
        '
        'Label_Note
        '
        Me.Label_Note.AutoSize = True
        Me.Label_Note.Location = New System.Drawing.Point(41, 162)
        Me.Label_Note.Name = "Label_Note"
        Me.Label_Note.Size = New System.Drawing.Size(43, 16)
        Me.Label_Note.TabIndex = 2
        Me.Label_Note.Text = "Notes"
        '
        'Label_Roll
        '
        Me.Label_Roll.AutoSize = True
        Me.Label_Roll.Location = New System.Drawing.Point(41, 111)
        Me.Label_Roll.Name = "Label_Roll"
        Me.Label_Roll.Size = New System.Drawing.Size(82, 16)
        Me.Label_Roll.TabIndex = 1
        Me.Label_Roll.Text = "Roll Number"
        '
        'Label_Name
        '
        Me.Label_Name.AutoSize = True
        Me.Label_Name.Location = New System.Drawing.Point(41, 60)
        Me.Label_Name.Name = "Label_Name"
        Me.Label_Name.Size = New System.Drawing.Size(44, 16)
        Me.Label_Name.TabIndex = 0
        Me.Label_Name.Text = "Name"
        '
        'T_Roll
        '
        Me.T_Roll.Location = New System.Drawing.Point(153, 107)
        Me.T_Roll.Name = "T_Roll"
        Me.T_Roll.Size = New System.Drawing.Size(248, 22)
        Me.T_Roll.TabIndex = 4
        Me.T_Roll.Text = "BSCS-F19-M63 @PUJC"
        '
        'T_Note
        '
        Me.T_Note.Location = New System.Drawing.Point(112, 157)
        Me.T_Note.Name = "T_Note"
        Me.T_Note.Size = New System.Drawing.Size(289, 22)
        Me.T_Note.TabIndex = 5
        Me.T_Note.Text = "Dont be so attached to who you are, in the present"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(44, 207)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(357, 50)
        Me.Button1.TabIndex = 6
        Me.Button1.Text = "Update Database"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'MyApp
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(564, 367)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "MyApp"
        Me.Text = "Simple DB Test Case"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label_Name As Label
    Friend WithEvents Label_Note As Label
    Friend WithEvents Label_Roll As Label
    Friend WithEvents T_Name As TextBox
    Friend WithEvents T_Note As TextBox
    Friend WithEvents T_Roll As TextBox
    Friend WithEvents Button1 As Button
End Class
