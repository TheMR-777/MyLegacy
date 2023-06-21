<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class VB22
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(VB22))
        Me.ImageBox = New System.Windows.Forms.PictureBox()
        Me.DisplayButton = New System.Windows.Forms.Button()
        CType(Me.ImageBox, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ImageBox
        '
        Me.ImageBox.Image = CType(resources.GetObject("ImageBox.Image"), System.Drawing.Image)
        Me.ImageBox.Location = New System.Drawing.Point(12, 12)
        Me.ImageBox.Name = "ImageBox"
        Me.ImageBox.Size = New System.Drawing.Size(451, 373)
        Me.ImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.ImageBox.TabIndex = 0
        Me.ImageBox.TabStop = False
        '
        'DisplayButton
        '
        Me.DisplayButton.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!)
        Me.DisplayButton.Location = New System.Drawing.Point(12, 391)
        Me.DisplayButton.Name = "DisplayButton"
        Me.DisplayButton.Size = New System.Drawing.Size(451, 47)
        Me.DisplayButton.TabIndex = 1
        Me.DisplayButton.Text = "Display Image"
        Me.DisplayButton.UseVisualStyleBackColor = True
        '
        'VB22
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(475, 450)
        Me.Controls.Add(Me.DisplayButton)
        Me.Controls.Add(Me.ImageBox)
        Me.Name = "VB22"
        Me.Text = "Playground"
        CType(Me.ImageBox, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ImageBox As PictureBox
    Friend WithEvents DisplayButton As Button
End Class
