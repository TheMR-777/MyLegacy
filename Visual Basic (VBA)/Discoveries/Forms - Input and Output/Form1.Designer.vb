<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MyForm
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
        Me.Txt_Name = New System.Windows.Forms.TextBox()
        Me.Txt_Mrks = New System.Windows.Forms.TextBox()
        Me.Txt_Roll = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Txt_CGPA = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'Txt_Name
        '
        Me.Txt_Name.Location = New System.Drawing.Point(260, 132)
        Me.Txt_Name.Name = "Txt_Name"
        Me.Txt_Name.Size = New System.Drawing.Size(268, 22)
        Me.Txt_Name.TabIndex = 1
        '
        'Txt_Mrks
        '
        Me.Txt_Mrks.Location = New System.Drawing.Point(260, 208)
        Me.Txt_Mrks.MaxLength = 3
        Me.Txt_Mrks.Name = "Txt_Mrks"
        Me.Txt_Mrks.Size = New System.Drawing.Size(268, 22)
        Me.Txt_Mrks.TabIndex = 2
        '
        'Txt_Roll
        '
        Me.Txt_Roll.Location = New System.Drawing.Point(260, 170)
        Me.Txt_Roll.Name = "Txt_Roll"
        Me.Txt_Roll.Size = New System.Drawing.Size(268, 22)
        Me.Txt_Roll.TabIndex = 3
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(172, 138)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(44, 16)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Name"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(172, 176)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(82, 16)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Roll Number"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(172, 214)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(44, 16)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Marks"
        '
        'Txt_CGPA
        '
        Me.Txt_CGPA.Location = New System.Drawing.Point(175, 251)
        Me.Txt_CGPA.Name = "Txt_CGPA"
        Me.Txt_CGPA.ReadOnly = True
        Me.Txt_CGPA.Size = New System.Drawing.Size(353, 22)
        Me.Txt_CGPA.TabIndex = 7
        Me.Txt_CGPA.Text = "CGPA"
        Me.Txt_CGPA.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'MyForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(701, 404)
        Me.Controls.Add(Me.Txt_CGPA)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Txt_Roll)
        Me.Controls.Add(Me.Txt_Mrks)
        Me.Controls.Add(Me.Txt_Name)
        Me.Name = "MyForm"
        Me.Text = "MyForm"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Txt_Name As TextBox
    Friend WithEvents Txt_Mrks As TextBox
    Friend WithEvents Txt_Roll As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Txt_CGPA As TextBox
End Class
