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
        Dim ReportDataSource2 As Microsoft.Reporting.WinForms.ReportDataSource = New Microsoft.Reporting.WinForms.ReportDataSource()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ReportApp_RDLC))
        Me.RecipeDBBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.MyRecipeDataSet = New LetsC.MyRecipeDataSet()
        Me.MyReportView = New Microsoft.Reporting.WinForms.ReportViewer()
        Me.RecipeDBTableAdapter = New LetsC.MyRecipeDataSetTableAdapters.RecipeDBTableAdapter()
        CType(Me.RecipeDBBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.MyRecipeDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'RecipeDBBindingSource
        '
        Me.RecipeDBBindingSource.DataMember = "RecipeDB"
        Me.RecipeDBBindingSource.DataSource = Me.MyRecipeDataSet
        '
        'MyRecipeDataSet
        '
        Me.MyRecipeDataSet.DataSetName = "MyRecipeDataSet"
        Me.MyRecipeDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'MyReportView
        '
        Me.MyReportView.Dock = System.Windows.Forms.DockStyle.Fill
        ReportDataSource2.Name = "MyData"
        ReportDataSource2.Value = Me.RecipeDBBindingSource
        Me.MyReportView.LocalReport.DataSources.Add(ReportDataSource2)
        Me.MyReportView.LocalReport.DisplayName = "The Report"
        Me.MyReportView.LocalReport.ReportEmbeddedResource = "LetsC.MyReport.rdlc"
        Me.MyReportView.Location = New System.Drawing.Point(0, 0)
        Me.MyReportView.Name = "MyReportView"
        Me.MyReportView.ServerReport.BearerToken = Nothing
        Me.MyReportView.Size = New System.Drawing.Size(947, 653)
        Me.MyReportView.TabIndex = 0
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
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "ReportApp_RDLC"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Recipes Report"
        CType(Me.RecipeDBBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.MyRecipeDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents MyReportView As Microsoft.Reporting.WinForms.ReportViewer
    Friend WithEvents MyRecipeDataSet As MyRecipeDataSet
    Friend WithEvents RecipeDBBindingSource As BindingSource
    Friend WithEvents RecipeDBTableAdapter As MyRecipeDataSetTableAdapters.RecipeDBTableAdapter
End Class
