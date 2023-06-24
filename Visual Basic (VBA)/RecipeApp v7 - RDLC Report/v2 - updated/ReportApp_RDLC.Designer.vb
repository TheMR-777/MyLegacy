<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ReportApp_RDLC
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
        Me.components = New System.ComponentModel.Container()
        Dim ReportDataSource1 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Me.MyReportView = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.MyRecipeDataSet = New LetsC.MyRecipeDataSet()
        Me.RecipeDBBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.RecipeDBTableAdapter = New LetsC.MyRecipeDataSetTableAdapters.RecipeDBTableAdapter()
        CType(Me.MyRecipeDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.RecipeDBBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'MyReportView
        '
        Me.MyReportView.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource1.Name = "MyData"
        ReportDataSource1.Value = Me.RecipeDBBindingSource
        Me.MyReportView.LocalReport.DataSources.Add(ReportDataSource1)
        Me.MyReportView.LocalReport.DisplayName = "The Report"
        Me.MyReportView.LocalReport.ReportEmbeddedResource = "LetsC.MyReport.rdlc"
        Me.MyReportView.Location = New System.Drawing.Point(0, 0)
        Me.MyReportView.Name = "MyReportView"
        Me.MyReportView.ServerReport.BearerToken = Nothing
        Me.MyReportView.Size = New System.Drawing.Size(947, 653)
        Me.MyReportView.TabIndex = 0
        '
        'MyRecipeDataSet
        '
        Me.MyRecipeDataSet.DataSetName = "MyRecipeDataSet"
        Me.MyRecipeDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'RecipeDBBindingSource
        '
        Me.RecipeDBBindingSource.DataMember = "RecipeDB"
        Me.RecipeDBBindingSource.DataSource = Me.MyRecipeDataSet
        '
        'RecipeDBTableAdapter
        '
        Me.RecipeDBTableAdapter.ClearBeforeFill = True
        '
        'ReportApp_RDLC
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(947, 653)
        Me.Controls.Add(Me.MyReportView)
        Me.Name = "ReportApp_RDLC"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "The RDLC Report"
        CType(Me.MyRecipeDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.RecipeDBBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents MyReportView As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents MyRecipeDataSet As MyRecipeDataSet
    Friend WithEvents RecipeDBBindingSource As BindingSource
    Friend WithEvents RecipeDBTableAdapter As MyRecipeDataSetTableAdapters.RecipeDBTableAdapter
End Class
