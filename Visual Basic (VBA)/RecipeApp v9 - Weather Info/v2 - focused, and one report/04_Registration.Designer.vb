<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Registration
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Registration))
        Me.F_NameL = New System.Windows.Forms.Label()
        Me.L_NameL = New System.Windows.Forms.Label()
        Me.EmailL = New System.Windows.Forms.Label()
        Me.PasswordL = New System.Windows.Forms.Label()
        Me.FirstName = New System.Windows.Forms.TextBox()
        Me.LastName = New System.Windows.Forms.TextBox()
        Me.Email = New System.Windows.Forms.TextBox()
        Me.Password = New System.Windows.Forms.TextBox()
        Me.RegisterButton = New System.Windows.Forms.Button()
        Me.CancelButton = New System.Windows.Forms.Button()
        Me.GoLogin = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'F_NameL
        '
        Me.F_NameL.AutoSize = True
        Me.F_NameL.Location = New System.Drawing.Point(85, 91)
        Me.F_NameL.Name = "F_NameL"
        Me.F_NameL.Size = New System.Drawing.Size(72, 16)
        Me.F_NameL.TabIndex = 0
        Me.F_NameL.Text = "First Name"
        '
        'L_NameL
        '
        Me.L_NameL.AutoSize = True
        Me.L_NameL.Location = New System.Drawing.Point(85, 135)
        Me.L_NameL.Name = "L_NameL"
        Me.L_NameL.Size = New System.Drawing.Size(72, 16)
        Me.L_NameL.TabIndex = 1
        Me.L_NameL.Text = "Last Name"
        '
        'EmailL
        '
        Me.EmailL.AutoSize = True
        Me.EmailL.Location = New System.Drawing.Point(85, 179)
        Me.EmailL.Name = "EmailL"
        Me.EmailL.Size = New System.Drawing.Size(41, 16)
        Me.EmailL.TabIndex = 2
        Me.EmailL.Text = "Email"
        '
        'PasswordL
        '
        Me.PasswordL.AutoSize = True
        Me.PasswordL.Location = New System.Drawing.Point(85, 223)
        Me.PasswordL.Name = "PasswordL"
        Me.PasswordL.Size = New System.Drawing.Size(67, 16)
        Me.PasswordL.TabIndex = 3
        Me.PasswordL.Text = "Password"
        '
        'FirstName
        '
        Me.FirstName.Location = New System.Drawing.Point(172, 88)
        Me.FirstName.Name = "FirstName"
        Me.FirstName.Size = New System.Drawing.Size(345, 22)
        Me.FirstName.TabIndex = 4
        '
        'LastName
        '
        Me.LastName.Location = New System.Drawing.Point(172, 132)
        Me.LastName.Name = "LastName"
        Me.LastName.Size = New System.Drawing.Size(345, 22)
        Me.LastName.TabIndex = 5
        '
        'Email
        '
        Me.Email.Location = New System.Drawing.Point(172, 176)
        Me.Email.Name = "Email"
        Me.Email.Size = New System.Drawing.Size(345, 22)
        Me.Email.TabIndex = 6
        '
        'Password
        '
        Me.Password.Location = New System.Drawing.Point(172, 220)
        Me.Password.Name = "Password"
        Me.Password.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.Password.Size = New System.Drawing.Size(345, 22)
        Me.Password.TabIndex = 7
        '
        'RegisterButton
        '
        Me.RegisterButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.RegisterButton.Location = New System.Drawing.Point(88, 269)
        Me.RegisterButton.Name = "RegisterButton"
        Me.RegisterButton.Size = New System.Drawing.Size(244, 49)
        Me.RegisterButton.TabIndex = 8
        Me.RegisterButton.Text = "Register"
        Me.RegisterButton.UseVisualStyleBackColor = True
        '
        'CancelButton
        '
        Me.CancelButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CancelButton.Location = New System.Drawing.Point(338, 269)
        Me.CancelButton.Name = "CancelButton"
        Me.CancelButton.Size = New System.Drawing.Size(179, 49)
        Me.CancelButton.TabIndex = 9
        Me.CancelButton.Text = "Cancel"
        Me.CancelButton.UseVisualStyleBackColor = True
        '
        'GoLogin
        '
        Me.GoLogin.Location = New System.Drawing.Point(338, 34)
        Me.GoLogin.Name = "GoLogin"
        Me.GoLogin.Size = New System.Drawing.Size(179, 33)
        Me.GoLogin.TabIndex = 10
        Me.GoLogin.Text = "Login instead"
        Me.GoLogin.UseVisualStyleBackColor = True
        '
        'Registration
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(602, 353)
        Me.Controls.Add(Me.GoLogin)
        Me.Controls.Add(Me.CancelButton)
        Me.Controls.Add(Me.RegisterButton)
        Me.Controls.Add(Me.Password)
        Me.Controls.Add(Me.Email)
        Me.Controls.Add(Me.LastName)
        Me.Controls.Add(Me.FirstName)
        Me.Controls.Add(Me.PasswordL)
        Me.Controls.Add(Me.EmailL)
        Me.Controls.Add(Me.L_NameL)
        Me.Controls.Add(Me.F_NameL)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Registration"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Registration"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents F_NameL As Label
    Friend WithEvents L_NameL As Label
    Friend WithEvents EmailL As Label
    Friend WithEvents PasswordL As Label
    Friend WithEvents FirstName As TextBox
    Friend WithEvents LastName As TextBox
    Friend WithEvents Email As TextBox
    Friend WithEvents Password As TextBox
    Friend WithEvents RegisterButton As Button
    Friend WithEvents CancelButton As Button
    Friend WithEvents GoLogin As Button
End Class
