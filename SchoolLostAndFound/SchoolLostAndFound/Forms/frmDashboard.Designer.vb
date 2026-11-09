<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDashboard
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
        Me.btnMatching = New Button()
        Me.btnUsers = New Button()
        Me.btnSettings = New Button()
        Me.pnlSidebar = New System.Windows.Forms.Panel()
        Me.btnActivityLogs = New System.Windows.Forms.Button()
        Me.btnReports = New System.Windows.Forms.Button()
        Me.btnClaims = New System.Windows.Forms.Button()
        Me.btnFoundItems = New System.Windows.Forms.Button()
        Me.btnLostItems = New System.Windows.Forms.Button()
        Me.btnCategories = New System.Windows.Forms.Button()
        Me.btnStudents = New System.Windows.Forms.Button()
        Me.btnDashboard = New System.Windows.Forms.Button()
        Me.pnlLogo = New System.Windows.Forms.Panel()
        Me.lblAppTitle = New System.Windows.Forms.Label()
        Me.pnlHeader = New System.Windows.Forms.Panel()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.btnLogout = New System.Windows.Forms.Button()
        Me.lblUserInfo = New System.Windows.Forms.Label()
        Me.pnlContent = New System.Windows.Forms.Panel()
        Me.pnlStats = New System.Windows.Forms.Panel()
        Me.grpCategories = New System.Windows.Forms.GroupBox()
        Me.lblTotalCategories = New System.Windows.Forms.Label()
        Me.grpStudents = New System.Windows.Forms.GroupBox()
        Me.lblTotalStudents = New System.Windows.Forms.Label()
        Me.grpClaims = New System.Windows.Forms.GroupBox()
        Me.lblPendingClaims = New System.Windows.Forms.Label()
        Me.grpFoundItems = New System.Windows.Forms.GroupBox()
        Me.lblTotalFoundItems = New System.Windows.Forms.Label()
        Me.grpLostItems = New System.Windows.Forms.GroupBox()
        Me.lblTotalLostItems = New System.Windows.Forms.Label()
        Me.lblWelcome = New System.Windows.Forms.Label()
        Me.pnlSidebar.SuspendLayout()
        Me.pnlLogo.SuspendLayout()
        Me.pnlHeader.SuspendLayout()
        Me.pnlContent.SuspendLayout()
        Me.pnlStats.SuspendLayout()
        Me.grpCategories.SuspendLayout()
        Me.grpStudents.SuspendLayout()
        Me.grpClaims.SuspendLayout()
        Me.grpFoundItems.SuspendLayout()
        Me.grpLostItems.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlSidebar
        '
        Me.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.btnMatching.Text="Item Matching"
        Me.btnMatching.Dock=DockStyle.Top
        Me.btnMatching.Height=42
        Me.btnMatching.ForeColor=Color.White
        Me.btnMatching.FlatStyle=FlatStyle.Flat
        Me.btnMatching.TextAlign=ContentAlignment.MiddleLeft
        Me.btnMatching.Padding=New Padding(20,0,0,0)
        Me.btnMatching.Name="btnMatching"
        Me.pnlSidebar.Controls.Add(Me.btnMatching)
        Me.btnUsers.Text="User Management"
        Me.btnUsers.Dock=DockStyle.Top
        Me.btnUsers.Height=42
        Me.btnUsers.ForeColor=Color.White
        Me.btnUsers.FlatStyle=FlatStyle.Flat
        Me.btnUsers.TextAlign=ContentAlignment.MiddleLeft
        Me.btnUsers.Padding=New Padding(20,0,0,0)
        Me.btnUsers.Name="btnUsers"
        Me.pnlSidebar.Controls.Add(Me.btnUsers)
        Me.btnSettings.Text="Settings / Password"
        Me.btnSettings.Dock=DockStyle.Top
        Me.btnSettings.Height=42
        Me.btnSettings.ForeColor=Color.White
        Me.btnSettings.FlatStyle=FlatStyle.Flat
        Me.btnSettings.TextAlign=ContentAlignment.MiddleLeft
        Me.btnSettings.Padding=New Padding(20,0,0,0)
        Me.btnSettings.Name="btnSettings"
        Me.pnlSidebar.Controls.Add(Me.btnSettings)
        Me.pnlSidebar.Controls.Add(Me.btnActivityLogs)
        Me.pnlSidebar.Controls.Add(Me.btnReports)
        Me.pnlSidebar.Controls.Add(Me.btnClaims)
        Me.pnlSidebar.Controls.Add(Me.btnFoundItems)
        Me.pnlSidebar.Controls.Add(Me.btnLostItems)
        Me.pnlSidebar.Controls.Add(Me.btnCategories)
        Me.pnlSidebar.Controls.Add(Me.btnStudents)
        Me.pnlSidebar.Controls.Add(Me.btnDashboard)
        Me.pnlSidebar.Controls.Add(Me.pnlLogo)
        Me.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left
        Me.pnlSidebar.Location = New System.Drawing.Point(0, 0)
        Me.pnlSidebar.Name = "pnlSidebar"
        Me.pnlSidebar.Size = New System.Drawing.Size(250, 600)
        Me.pnlSidebar.TabIndex = 0
        Me.pnlSidebar.AutoScroll = True
        Me.MinimumSize = New Size(1024, 640)
        '
        'btnActivityLogs
        '
        Me.btnActivityLogs.Dock = System.Windows.Forms.DockStyle.Top
        Me.btnActivityLogs.FlatAppearance.BorderSize = 0
        Me.btnActivityLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnActivityLogs.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnActivityLogs.ForeColor = System.Drawing.Color.White
        Me.btnActivityLogs.Location = New System.Drawing.Point(0, 430)
        Me.btnActivityLogs.Name = "btnActivityLogs"
        Me.btnActivityLogs.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.btnActivityLogs.Size = New System.Drawing.Size(250, 42)
        Me.btnActivityLogs.TabIndex = 8
        Me.btnActivityLogs.Text = "Activity Logs"
        Me.btnActivityLogs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnActivityLogs.UseVisualStyleBackColor = True
        '
        'btnReports
        '
        Me.btnReports.Dock = System.Windows.Forms.DockStyle.Top
        Me.btnReports.FlatAppearance.BorderSize = 0
        Me.btnReports.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnReports.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnReports.ForeColor = System.Drawing.Color.White
        Me.btnReports.Location = New System.Drawing.Point(0, 380)
        Me.btnReports.Name = "btnReports"
        Me.btnReports.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.btnReports.Size = New System.Drawing.Size(250, 42)
        Me.btnReports.TabIndex = 7
        Me.btnReports.Text = "Reports"
        Me.btnReports.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnReports.UseVisualStyleBackColor = True
        '
        'btnClaims
        '
        Me.btnClaims.Dock = System.Windows.Forms.DockStyle.Top
        Me.btnClaims.FlatAppearance.BorderSize = 0
        Me.btnClaims.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClaims.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnClaims.ForeColor = System.Drawing.Color.White
        Me.btnClaims.Location = New System.Drawing.Point(0, 330)
        Me.btnClaims.Name = "btnClaims"
        Me.btnClaims.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.btnClaims.Size = New System.Drawing.Size(250, 42)
        Me.btnClaims.TabIndex = 6
        Me.btnClaims.Text = "Claims"
        Me.btnClaims.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnClaims.UseVisualStyleBackColor = True
        '
        'btnFoundItems
        '
        Me.btnFoundItems.Dock = System.Windows.Forms.DockStyle.Top
        Me.btnFoundItems.FlatAppearance.BorderSize = 0
        Me.btnFoundItems.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnFoundItems.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFoundItems.ForeColor = System.Drawing.Color.White
        Me.btnFoundItems.Location = New System.Drawing.Point(0, 280)
        Me.btnFoundItems.Name = "btnFoundItems"
        Me.btnFoundItems.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.btnFoundItems.Size = New System.Drawing.Size(250, 42)
        Me.btnFoundItems.TabIndex = 5
        Me.btnFoundItems.Text = "Found Items"
        Me.btnFoundItems.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnFoundItems.UseVisualStyleBackColor = True
        '
        'btnLostItems
        '
        Me.btnLostItems.Dock = System.Windows.Forms.DockStyle.Top
        Me.btnLostItems.FlatAppearance.BorderSize = 0
        Me.btnLostItems.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnLostItems.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnLostItems.ForeColor = System.Drawing.Color.White
        Me.btnLostItems.Location = New System.Drawing.Point(0, 230)
        Me.btnLostItems.Name = "btnLostItems"
        Me.btnLostItems.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.btnLostItems.Size = New System.Drawing.Size(250, 42)
        Me.btnLostItems.TabIndex = 4
        Me.btnLostItems.Text = "Lost Items"
        Me.btnLostItems.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnLostItems.UseVisualStyleBackColor = True
        '
        'btnCategories
        '
        Me.btnCategories.Dock = System.Windows.Forms.DockStyle.Top
        Me.btnCategories.FlatAppearance.BorderSize = 0
        Me.btnCategories.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCategories.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCategories.ForeColor = System.Drawing.Color.White
        Me.btnCategories.Location = New System.Drawing.Point(0, 180)
        Me.btnCategories.Name = "btnCategories"
        Me.btnCategories.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.btnCategories.Size = New System.Drawing.Size(250, 42)
        Me.btnCategories.TabIndex = 3
        Me.btnCategories.Text = "Categories"
        Me.btnCategories.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCategories.UseVisualStyleBackColor = True
        '
        'btnStudents
        '
        Me.btnStudents.Dock = System.Windows.Forms.DockStyle.Top
        Me.btnStudents.FlatAppearance.BorderSize = 0
        Me.btnStudents.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnStudents.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnStudents.ForeColor = System.Drawing.Color.White
        Me.btnStudents.Location = New System.Drawing.Point(0, 130)
        Me.btnStudents.Name = "btnStudents"
        Me.btnStudents.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.btnStudents.Size = New System.Drawing.Size(250, 42)
        Me.btnStudents.TabIndex = 2
        Me.btnStudents.Text = "Students"
        Me.btnStudents.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnStudents.UseVisualStyleBackColor = True
        '
        'btnDashboard
        '
        Me.btnDashboard.BackColor = System.Drawing.Color.FromArgb(CType(CType(52, Byte), Integer), CType(CType(152, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.btnDashboard.Dock = System.Windows.Forms.DockStyle.Top
        Me.btnDashboard.FlatAppearance.BorderSize = 0
        Me.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnDashboard.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDashboard.ForeColor = System.Drawing.Color.White
        Me.btnDashboard.Location = New System.Drawing.Point(0, 80)
        Me.btnDashboard.Name = "btnDashboard"
        Me.btnDashboard.Padding = New System.Windows.Forms.Padding(20, 0, 0, 0)
        Me.btnDashboard.Size = New System.Drawing.Size(250, 42)
        Me.btnDashboard.TabIndex = 1
        Me.btnDashboard.Text = "Dashboard"
        Me.btnDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnDashboard.UseVisualStyleBackColor = False
        '
        'pnlLogo
        '
        Me.pnlLogo.BackColor = System.Drawing.Color.FromArgb(CType(CType(33, Byte), Integer), CType(CType(47, Byte), Integer), CType(CType(61, Byte), Integer))
        Me.pnlLogo.Controls.Add(Me.lblAppTitle)
        Me.pnlLogo.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlLogo.Location = New System.Drawing.Point(0, 0)
        Me.pnlLogo.Name = "pnlLogo"
        Me.pnlLogo.Size = New System.Drawing.Size(250, 80)
        Me.pnlLogo.TabIndex = 0
        '
        'lblAppTitle
        '
        Me.lblAppTitle.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblAppTitle.Font = New System.Drawing.Font("Segoe UI", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAppTitle.ForeColor = System.Drawing.Color.White
        Me.lblAppTitle.Location = New System.Drawing.Point(0, 0)
        Me.lblAppTitle.Name = "lblAppTitle"
        Me.lblAppTitle.Size = New System.Drawing.Size(250, 80)
        Me.lblAppTitle.TabIndex = 0
        Me.lblAppTitle.Text = "School Lost && Found"
        Me.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnlHeader
        '
        Me.pnlHeader.BackColor = System.Drawing.Color.White
        Me.pnlHeader.Controls.Add(Me.btnExit)
        Me.pnlHeader.Controls.Add(Me.btnLogout)
        Me.pnlHeader.Controls.Add(Me.lblUserInfo)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeader.Location = New System.Drawing.Point(250, 0)
        Me.pnlHeader.Name = "pnlHeader"
        Me.pnlHeader.Size = New System.Drawing.Size(758, 80)
        Me.pnlHeader.TabIndex = 1
        '
        'btnExit
        '
        Me.btnExit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnExit.BackColor = System.Drawing.Color.FromArgb(CType(CType(231, Byte), Integer), CType(CType(76, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.btnExit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnExit.FlatAppearance.BorderSize = 0
        Me.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnExit.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnExit.ForeColor = System.Drawing.Color.White
        Me.btnExit.Location = New System.Drawing.Point(656, 25)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(80, 30)
        Me.btnExit.TabIndex = 2
        Me.btnExit.Text = "Exit"
        Me.btnExit.UseVisualStyleBackColor = False
        '
        'btnLogout
        '
        Me.btnLogout.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnLogout.BackColor = System.Drawing.Color.FromArgb(CType(CType(149, Byte), Integer), CType(CType(165, Byte), Integer), CType(CType(166, Byte), Integer))
        Me.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnLogout.FlatAppearance.BorderSize = 0
        Me.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnLogout.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnLogout.ForeColor = System.Drawing.Color.White
        Me.btnLogout.Location = New System.Drawing.Point(560, 25)
        Me.btnLogout.Name = "btnLogout"
        Me.btnLogout.Size = New System.Drawing.Size(80, 30)
        Me.btnLogout.TabIndex = 1
        Me.btnLogout.Text = "Logout"
        Me.btnLogout.UseVisualStyleBackColor = False
        '
        'lblUserInfo
        '
        Me.lblUserInfo.AutoSize = True
        Me.lblUserInfo.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUserInfo.Location = New System.Drawing.Point(30, 30)
        Me.lblUserInfo.Name = "lblUserInfo"
        Me.lblUserInfo.Size = New System.Drawing.Size(126, 20)
        Me.lblUserInfo.TabIndex = 0
        Me.lblUserInfo.Text = "Welcome, User (Role)"
        '
        'pnlContent
        '
        Me.pnlContent.BackColor = System.Drawing.Color.FromArgb(CType(CType(236, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(241, Byte), Integer))
        Me.pnlContent.Controls.Add(Me.pnlStats)
        Me.pnlContent.Controls.Add(Me.lblWelcome)
        Me.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlContent.Location = New System.Drawing.Point(250, 80)
        Me.pnlContent.Name = "pnlContent"
        Me.pnlContent.Size = New System.Drawing.Size(758, 520)
        Me.pnlContent.TabIndex = 2
        '
        'pnlStats
        '
        Me.pnlStats.Controls.Add(Me.grpCategories)
        Me.pnlStats.Controls.Add(Me.grpStudents)
        Me.pnlStats.Controls.Add(Me.grpClaims)
        Me.pnlStats.Controls.Add(Me.grpFoundItems)
        Me.pnlStats.Controls.Add(Me.grpLostItems)
        Me.pnlStats.Location = New System.Drawing.Point(34, 100)
        Me.pnlStats.Name = "pnlStats"
        Me.pnlStats.Size = New System.Drawing.Size(692, 360)
        Me.pnlStats.TabIndex = 1
        '
        'grpCategories
        '
        Me.grpCategories.Controls.Add(Me.lblTotalCategories)
        Me.grpCategories.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grpCategories.Location = New System.Drawing.Point(240, 180)
        Me.grpCategories.Name = "grpCategories"
        Me.grpCategories.Size = New System.Drawing.Size(200, 120)
        Me.grpCategories.TabIndex = 4
        Me.grpCategories.TabStop = False
        Me.grpCategories.Text = "Total Categories"
        '
        'lblTotalCategories
        '
        Me.lblTotalCategories.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTotalCategories.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotalCategories.ForeColor = System.Drawing.Color.FromArgb(CType(CType(155, Byte), Integer), CType(CType(89, Byte), Integer), CType(CType(182, Byte), Integer))
        Me.lblTotalCategories.Location = New System.Drawing.Point(3, 23)
        Me.lblTotalCategories.Name = "lblTotalCategories"
        Me.lblTotalCategories.Size = New System.Drawing.Size(194, 94)
        Me.lblTotalCategories.TabIndex = 0
        Me.lblTotalCategories.Text = "0"
        Me.lblTotalCategories.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'grpStudents
        '
        Me.grpStudents.Controls.Add(Me.lblTotalStudents)
        Me.grpStudents.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grpStudents.Location = New System.Drawing.Point(20, 180)
        Me.grpStudents.Name = "grpStudents"
        Me.grpStudents.Size = New System.Drawing.Size(200, 120)
        Me.grpStudents.TabIndex = 3
        Me.grpStudents.TabStop = False
        Me.grpStudents.Text = "Total Students"
        '
        'lblTotalStudents
        '
        Me.lblTotalStudents.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTotalStudents.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotalStudents.ForeColor = System.Drawing.Color.FromArgb(CType(CType(52, Byte), Integer), CType(CType(152, Byte), Integer), CType(CType(219, Byte), Integer))
        Me.lblTotalStudents.Location = New System.Drawing.Point(3, 23)
        Me.lblTotalStudents.Name = "lblTotalStudents"
        Me.lblTotalStudents.Size = New System.Drawing.Size(194, 94)
        Me.lblTotalStudents.TabIndex = 0
        Me.lblTotalStudents.Text = "0"
        Me.lblTotalStudents.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'grpClaims
        '
        Me.grpClaims.Controls.Add(Me.lblPendingClaims)
        Me.grpClaims.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grpClaims.Location = New System.Drawing.Point(460, 30)
        Me.grpClaims.Name = "grpClaims"
        Me.grpClaims.Size = New System.Drawing.Size(200, 120)
        Me.grpClaims.TabIndex = 2
        Me.grpClaims.TabStop = False
        Me.grpClaims.Text = "Pending Claims"
        '
        'lblPendingClaims
        '
        Me.lblPendingClaims.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblPendingClaims.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPendingClaims.ForeColor = System.Drawing.Color.FromArgb(CType(CType(243, Byte), Integer), CType(CType(156, Byte), Integer), CType(CType(18, Byte), Integer))
        Me.lblPendingClaims.Location = New System.Drawing.Point(3, 23)
        Me.lblPendingClaims.Name = "lblPendingClaims"
        Me.lblPendingClaims.Size = New System.Drawing.Size(194, 94)
        Me.lblPendingClaims.TabIndex = 0
        Me.lblPendingClaims.Text = "0"
        Me.lblPendingClaims.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'grpFoundItems
        '
        Me.grpFoundItems.Controls.Add(Me.lblTotalFoundItems)
        Me.grpFoundItems.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grpFoundItems.Location = New System.Drawing.Point(240, 30)
        Me.grpFoundItems.Name = "grpFoundItems"
        Me.grpFoundItems.Size = New System.Drawing.Size(200, 120)
        Me.grpFoundItems.TabIndex = 1
        Me.grpFoundItems.TabStop = False
        Me.grpFoundItems.Text = "Total Found Items"
        '
        'lblTotalFoundItems
        '
        Me.lblTotalFoundItems.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTotalFoundItems.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotalFoundItems.ForeColor = System.Drawing.Color.FromArgb(CType(CType(46, Byte), Integer), CType(CType(204, Byte), Integer), CType(CType(113, Byte), Integer))
        Me.lblTotalFoundItems.Location = New System.Drawing.Point(3, 23)
        Me.lblTotalFoundItems.Name = "lblTotalFoundItems"
        Me.lblTotalFoundItems.Size = New System.Drawing.Size(194, 94)
        Me.lblTotalFoundItems.TabIndex = 0
        Me.lblTotalFoundItems.Text = "0"
        Me.lblTotalFoundItems.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'grpLostItems
        '
        Me.grpLostItems.Controls.Add(Me.lblTotalLostItems)
        Me.grpLostItems.Font = New System.Drawing.Font("Segoe UI", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grpLostItems.Location = New System.Drawing.Point(20, 30)
        Me.grpLostItems.Name = "grpLostItems"
        Me.grpLostItems.Size = New System.Drawing.Size(200, 120)
        Me.grpLostItems.TabIndex = 0
        Me.grpLostItems.TabStop = False
        Me.grpLostItems.Text = "Total Lost Items"
        '
        'lblTotalLostItems
        '
        Me.lblTotalLostItems.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTotalLostItems.Font = New System.Drawing.Font("Segoe UI", 24.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotalLostItems.ForeColor = System.Drawing.Color.FromArgb(CType(CType(231, Byte), Integer), CType(CType(76, Byte), Integer), CType(CType(60, Byte), Integer))
        Me.lblTotalLostItems.Location = New System.Drawing.Point(3, 23)
        Me.lblTotalLostItems.Name = "lblTotalLostItems"
        Me.lblTotalLostItems.Size = New System.Drawing.Size(194, 94)
        Me.lblTotalLostItems.TabIndex = 0
        Me.lblTotalLostItems.Text = "0"
        Me.lblTotalLostItems.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblWelcome
        '
        Me.lblWelcome.AutoSize = True
        Me.lblWelcome.Font = New System.Drawing.Font("Segoe UI", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(CType(CType(44, Byte), Integer), CType(CType(62, Byte), Integer), CType(CType(80, Byte), Integer))
        Me.lblWelcome.Location = New System.Drawing.Point(27, 30)
        Me.lblWelcome.Name = "lblWelcome"
        Me.lblWelcome.Size = New System.Drawing.Size(273, 37)
        Me.lblWelcome.TabIndex = 0
        Me.lblWelcome.Text = "Dashboard Overview"
        '
        'frmDashboard
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1008, 600)
        Me.Controls.Add(Me.pnlContent)
        Me.Controls.Add(Me.pnlHeader)
        Me.Controls.Add(Me.pnlSidebar)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Name = "frmDashboard"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Dashboard - School Lost & Found"
        Me.pnlSidebar.ResumeLayout(False)
        Me.pnlLogo.ResumeLayout(False)
        Me.pnlHeader.ResumeLayout(False)
        Me.pnlHeader.PerformLayout()
        Me.pnlContent.ResumeLayout(False)
        Me.pnlContent.PerformLayout()
        Me.pnlStats.ResumeLayout(False)
        Me.grpCategories.ResumeLayout(False)
        Me.grpStudents.ResumeLayout(False)
        Me.grpClaims.ResumeLayout(False)
        Me.grpFoundItems.ResumeLayout(False)
        Me.grpLostItems.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnMatching As Button
    Friend WithEvents btnUsers As Button
    Friend WithEvents btnSettings As Button
    Friend WithEvents pnlSidebar As Panel
    Friend WithEvents btnActivityLogs As Button
    Friend WithEvents btnReports As Button
    Friend WithEvents btnClaims As Button
    Friend WithEvents btnFoundItems As Button
    Friend WithEvents btnLostItems As Button
    Friend WithEvents btnCategories As Button
    Friend WithEvents btnStudents As Button
    Friend WithEvents btnDashboard As Button
    Friend WithEvents pnlLogo As Panel
    Friend WithEvents lblAppTitle As Label
    Friend WithEvents pnlHeader As Panel
    Friend WithEvents lblUserInfo As Label
    Friend WithEvents btnExit As Button
    Friend WithEvents btnLogout As Button
    Friend WithEvents pnlContent As Panel
    Friend WithEvents lblWelcome As Label
    Friend WithEvents pnlStats As Panel
    Friend WithEvents grpLostItems As GroupBox
    Friend WithEvents lblTotalLostItems As Label
    Friend WithEvents grpFoundItems As GroupBox
    Friend WithEvents lblTotalFoundItems As Label
    Friend WithEvents grpClaims As GroupBox
    Friend WithEvents lblPendingClaims As Label
    Friend WithEvents grpStudents As GroupBox
    Friend WithEvents lblTotalStudents As Label
    Friend WithEvents grpCategories As GroupBox
    Friend WithEvents lblTotalCategories As Label
End Class
