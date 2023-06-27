<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Login
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
        Me.CancelButton = New System.Windows.Forms.Button()
        Me.LoginButton = New System.Windows.Forms.Button()
        Me.Password = New System.Windows.Forms.TextBox()
        Me.Email = New System.Windows.Forms.TextBox()
        Me.L_Password = New System.Windows.Forms.Label()
        Me.L_Email = New System.Windows.Forms.Label()
        Me.Register = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'CancelButton
        '
        Me.CancelButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CancelButton.Location = New System.Drawing.Point(338, 173)
        Me.CancelButton.Name = "CancelButton"
        Me.CancelButton.Size = New System.Drawing.Size(179, 49)
        Me.CancelButton.TabIndex = 19
        Me.CancelButton.Text = "Cancel"
        Me.CancelButton.UseVisualStyleBackColor = True
        '
        'LoginButton
        '
        Me.LoginButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LoginButton.Location = New System.Drawing.Point(88, 173)
        Me.LoginButton.Name = "LoginButton"
        Me.LoginButton.Size = New System.Drawing.Size(244, 49)
        Me.LoginButton.TabIndex = 18
        Me.LoginButton.Text = "Login"
        Me.LoginButton.UseVisualStyleBackColor = True
        '
        'Password
        '
        Me.Password.Location = New System.Drawing.Point(172, 124)
        Me.Password.Name = "Password"
        Me.Password.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.Password.Size = New System.Drawing.Size(345, 22)
        Me.Password.TabIndex = 17
        '
        'Email
        '
        Me.Email.Location = New System.Drawing.Point(172, 80)
        Me.Email.Name = "Email"
        Me.Email.Size = New System.Drawing.Size(345, 22)
        Me.Email.TabIndex = 16
        '
        'L_Password
        '
        Me.L_Password.AutoSize = True
        Me.L_Password.Location = New System.Drawing.Point(85, 127)
        Me.L_Password.Name = "L_Password"
        Me.L_Password.Size = New System.Drawing.Size(67, 16)
        Me.L_Password.TabIndex = 13
        Me.L_Password.Text = "Password"
        '
        'L_Email
        '
        Me.L_Email.AutoSize = True
        Me.L_Email.Location = New System.Drawing.Point(85, 83)
        Me.L_Email.Name = "L_Email"
        Me.L_Email.Size = New System.Drawing.Size(41, 16)
        Me.L_Email.TabIndex = 12
        Me.L_Email.Text = "Email"
        '
        'Register
        '
        Me.Register.Location = New System.Drawing.Point(435, 30)
        Me.Register.Name = "Register"
        Me.Register.Size = New System.Drawing.Size(81, 33)
        Me.Register.TabIndex = 20
        Me.Register.Text = "Register"
        Me.Register.UseVisualStyleBackColor = True
        '
        'Login
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(602, 253)
        Me.Controls.Add(Me.Register)
        Me.Controls.Add(Me.CancelButton)
        Me.Controls.Add(Me.LoginButton)
        Me.Controls.Add(Me.Password)
        Me.Controls.Add(Me.Email)
        Me.Controls.Add(Me.L_Password)
        Me.Controls.Add(Me.L_Email)
        Me.Name = "Login"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Login"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents CancelButton As Button
    Friend WithEvents LoginButton As Button
    Friend WithEvents Password As TextBox
    Friend WithEvents Email As TextBox
    Friend WithEvents L_Password As Label
    Friend WithEvents L_Email As Label
    Friend WithEvents Register As Button
End Class
