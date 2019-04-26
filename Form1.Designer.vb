<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form은 Dispose를 재정의하여 구성 요소 목록을 정리합니다.
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

    'Windows Form 디자이너에 필요합니다.
    Private components As System.ComponentModel.IContainer

    '참고: 다음 프로시저는 Windows Form 디자이너에 필요합니다.
    '수정하려면 Windows Form 디자이너를 사용하십시오.  
    '코드 편집기를 사용하여 수정하지 마십시오.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.btnStart = New System.Windows.Forms.Button()
        Me.btnStop = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.picPreview = New System.Windows.Forms.PictureBox()
        Me.lstDevices = New System.Windows.Forms.ListBox()
        Me.sfdImage = New System.Windows.Forms.SaveFileDialog()
        Me.picTCap = New System.Windows.Forms.PictureBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.btnInfo = New System.Windows.Forms.Button()
        Me.picLsr = New System.Windows.Forms.PictureBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.btnSample = New System.Windows.Forms.Button()
        Me.ofd = New System.Windows.Forms.OpenFileDialog()
        Me.picSample = New System.Windows.Forms.PictureBox()
        Me.picMatch = New System.Windows.Forms.PictureBox()
        Me.cbLsr = New System.Windows.Forms.CheckBox()
        Me.cbTM = New System.Windows.Forms.CheckBox()
        Me.radGB = New System.Windows.Forms.RadioButton()
        Me.radEB = New System.Windows.Forms.RadioButton()
        Me.btnOb = New System.Windows.Forms.Button()
        CType(Me.picPreview, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picTCap, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picLsr, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picSample, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picMatch, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnStart
        '
        Me.btnStart.Enabled = False
        Me.btnStart.Location = New System.Drawing.Point(12, 160)
        Me.btnStart.Name = "btnStart"
        Me.btnStart.Size = New System.Drawing.Size(75, 23)
        Me.btnStart.TabIndex = 0
        Me.btnStart.Text = "Start"
        Me.btnStart.UseVisualStyleBackColor = True
        '
        'btnStop
        '
        Me.btnStop.Location = New System.Drawing.Point(12, 189)
        Me.btnStop.Name = "btnStop"
        Me.btnStop.Size = New System.Drawing.Size(75, 23)
        Me.btnStop.TabIndex = 1
        Me.btnStop.Text = "Stop"
        Me.btnStop.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(12, 218)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(75, 23)
        Me.btnSave.TabIndex = 2
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'picPreview
        '
        Me.picPreview.BackColor = System.Drawing.Color.White
        Me.picPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picPreview.Location = New System.Drawing.Point(12, 12)
        Me.picPreview.Name = "picPreview"
        Me.picPreview.Size = New System.Drawing.Size(108, 96)
        Me.picPreview.TabIndex = 3
        Me.picPreview.TabStop = False
        '
        'lstDevices
        '
        Me.lstDevices.FormattingEnabled = True
        Me.lstDevices.ItemHeight = 12
        Me.lstDevices.Location = New System.Drawing.Point(12, 276)
        Me.lstDevices.Name = "lstDevices"
        Me.lstDevices.Size = New System.Drawing.Size(85, 28)
        Me.lstDevices.TabIndex = 4
        '
        'picTCap
        '
        Me.picTCap.BackColor = System.Drawing.Color.White
        Me.picTCap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picTCap.Location = New System.Drawing.Point(126, 12)
        Me.picTCap.Name = "picTCap"
        Me.picTCap.Size = New System.Drawing.Size(400, 300)
        Me.picTCap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picTCap.TabIndex = 5
        Me.picTCap.TabStop = False
        '
        'Timer1
        '
        '
        'btnInfo
        '
        Me.btnInfo.Location = New System.Drawing.Point(12, 247)
        Me.btnInfo.Name = "btnInfo"
        Me.btnInfo.Size = New System.Drawing.Size(85, 23)
        Me.btnInfo.TabIndex = 8
        Me.btnInfo.Text = "해상도&&정보"
        Me.btnInfo.UseVisualStyleBackColor = True
        '
        'picLsr
        '
        Me.picLsr.BackColor = System.Drawing.Color.White
        Me.picLsr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picLsr.Location = New System.Drawing.Point(126, 318)
        Me.picLsr.Name = "picLsr"
        Me.picLsr.Size = New System.Drawing.Size(200, 300)
        Me.picLsr.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picLsr.TabIndex = 10
        Me.picLsr.TabStop = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(332, 318)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(42, 12)
        Me.Label1.TabIndex = 11
        Me.Label1.Text = "Label1"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(332, 330)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(42, 12)
        Me.Label2.TabIndex = 12
        Me.Label2.Text = "Label2"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(530, 525)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(42, 12)
        Me.Label3.TabIndex = 13
        Me.Label3.Text = "Label3"
        Me.Label3.Visible = False
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(530, 537)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(42, 12)
        Me.Label4.TabIndex = 14
        Me.Label4.Text = "Label4"
        Me.Label4.Visible = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(530, 549)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(42, 12)
        Me.Label5.TabIndex = 15
        Me.Label5.Text = "Label5"
        Me.Label5.Visible = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(530, 561)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(42, 12)
        Me.Label6.TabIndex = 16
        Me.Label6.Text = "Label6"
        Me.Label6.Visible = False
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(530, 573)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(42, 12)
        Me.Label7.TabIndex = 17
        Me.Label7.Text = "Label7"
        Me.Label7.Visible = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(530, 585)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(42, 12)
        Me.Label8.TabIndex = 18
        Me.Label8.Text = "Label8"
        Me.Label8.Visible = False
        '
        'btnSample
        '
        Me.btnSample.Location = New System.Drawing.Point(664, 319)
        Me.btnSample.Name = "btnSample"
        Me.btnSample.Size = New System.Drawing.Size(97, 23)
        Me.btnSample.TabIndex = 36
        Me.btnSample.Text = "SampleSelect"
        Me.btnSample.UseVisualStyleBackColor = True
        '
        'ofd
        '
        Me.ofd.FileName = "OpenFileDialog1"
        '
        'picSample
        '
        Me.picSample.BackColor = System.Drawing.Color.White
        Me.picSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picSample.Location = New System.Drawing.Point(532, 318)
        Me.picSample.Name = "picSample"
        Me.picSample.Size = New System.Drawing.Size(126, 108)
        Me.picSample.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picSample.TabIndex = 37
        Me.picSample.TabStop = False
        '
        'picMatch
        '
        Me.picMatch.BackColor = System.Drawing.Color.White
        Me.picMatch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picMatch.Location = New System.Drawing.Point(532, 12)
        Me.picMatch.Name = "picMatch"
        Me.picMatch.Size = New System.Drawing.Size(400, 300)
        Me.picMatch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picMatch.TabIndex = 9
        Me.picMatch.TabStop = False
        '
        'cbLsr
        '
        Me.cbLsr.AutoSize = True
        Me.cbLsr.Location = New System.Drawing.Point(12, 114)
        Me.cbLsr.Name = "cbLsr"
        Me.cbLsr.Size = New System.Drawing.Size(56, 16)
        Me.cbLsr.TabIndex = 38
        Me.cbLsr.Text = "Laser"
        Me.cbLsr.UseVisualStyleBackColor = True
        '
        'cbTM
        '
        Me.cbTM.AutoSize = True
        Me.cbTM.Location = New System.Drawing.Point(12, 136)
        Me.cbTM.Name = "cbTM"
        Me.cbTM.Size = New System.Drawing.Size(43, 16)
        Me.cbTM.TabIndex = 39
        Me.cbTM.Text = "TM"
        Me.cbTM.UseVisualStyleBackColor = True
        '
        'radGB
        '
        Me.radGB.AutoSize = True
        Me.radGB.Checked = True
        Me.radGB.Location = New System.Drawing.Point(532, 438)
        Me.radGB.Name = "radGB"
        Me.radGB.Size = New System.Drawing.Size(124, 16)
        Me.radGB.TabIndex = 40
        Me.radGB.TabStop = True
        Me.radGB.Text = "Gray Based(SAD)"
        Me.radGB.UseVisualStyleBackColor = True
        '
        'radEB
        '
        Me.radEB.AutoSize = True
        Me.radEB.Location = New System.Drawing.Point(532, 462)
        Me.radEB.Name = "radEB"
        Me.radEB.Size = New System.Drawing.Size(92, 16)
        Me.radEB.TabIndex = 41
        Me.radEB.TabStop = True
        Me.radEB.Text = "Edge Based"
        Me.radEB.UseVisualStyleBackColor = True
        '
        'btnOb
        '
        Me.btnOb.Location = New System.Drawing.Point(835, 319)
        Me.btnOb.Name = "btnOb"
        Me.btnOb.Size = New System.Drawing.Size(97, 36)
        Me.btnOb.TabIndex = 42
        Me.btnOb.Text = "Obstacle Detect"
        Me.btnOb.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(954, 628)
        Me.Controls.Add(Me.btnOb)
        Me.Controls.Add(Me.radEB)
        Me.Controls.Add(Me.radGB)
        Me.Controls.Add(Me.cbTM)
        Me.Controls.Add(Me.cbLsr)
        Me.Controls.Add(Me.picSample)
        Me.Controls.Add(Me.btnSample)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.picLsr)
        Me.Controls.Add(Me.picMatch)
        Me.Controls.Add(Me.btnInfo)
        Me.Controls.Add(Me.picTCap)
        Me.Controls.Add(Me.lstDevices)
        Me.Controls.Add(Me.picPreview)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.btnStop)
        Me.Controls.Add(Me.btnStart)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.picPreview, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picTCap, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picLsr, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picSample, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picMatch, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnStart As System.Windows.Forms.Button
    Friend WithEvents btnStop As System.Windows.Forms.Button
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents picPreview As System.Windows.Forms.PictureBox
    Friend WithEvents lstDevices As System.Windows.Forms.ListBox
    Friend WithEvents sfdImage As System.Windows.Forms.SaveFileDialog
    Friend WithEvents picTCap As System.Windows.Forms.PictureBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents btnInfo As System.Windows.Forms.Button
    Friend WithEvents picLsr As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents btnSample As System.Windows.Forms.Button
    Friend WithEvents ofd As System.Windows.Forms.OpenFileDialog
    Friend WithEvents picSample As System.Windows.Forms.PictureBox
    Friend WithEvents picMatch As System.Windows.Forms.PictureBox
    Friend WithEvents cbLsr As System.Windows.Forms.CheckBox
    Friend WithEvents cbTM As System.Windows.Forms.CheckBox
    Friend WithEvents radGB As System.Windows.Forms.RadioButton
    Friend WithEvents radEB As System.Windows.Forms.RadioButton
    Friend WithEvents btnOb As System.Windows.Forms.Button

End Class
