Imports System.IO
Imports System.Text
Imports MySql.Data.MySqlClient
Public Class Connection
    Dim ValidLocalConnection As Boolean = False
    Dim ValidCloudConnection As Boolean = False
    Dim LocalConnectionPath As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\Innovention\user.config"

    Dim IFLoadedLocalCon As Boolean = True
    Dim IfLoadedCloudCon As Boolean = True

    Dim Autobackup As Boolean = False

    Dim Sql As String = ""
    Private Sub Connection_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            TabControl2.TabPages(0).Text = "Connection Settings"
            TabControl2.TabPages(1).Text = "Additional Settings"
            TabControl2.TabPages(2).Text = "Local Reset Table"

            LoadConn()
            LoadCloudConn()
            LoadAutoBackup()
            LoadAdditionalSettings()
            LoadDefaultSettingsDev()
            LoadDefaultSettingsAdd()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
#Region "LoadCred"
    Private Sub LoadConn()
        Try
            If LocalConnectionPath <> "" Then
                If System.IO.File.Exists(LocalConnectionPath) Then
                    'The File exists 
                    ValidLocalConnection = True
                    Dim CreateConnString As String = ""
                    Dim filename As String = String.Empty
                    Dim TextLine As String = ""
                    Dim objReader As New System.IO.StreamReader(LocalConnectionPath)
                    Dim lineCount As Integer
                    Do While objReader.Peek() <> -1
                        TextLine = objReader.ReadLine()
                        If lineCount = 0 Then
                            TextBoxLocalServer.Text = ConvertB64ToString(RemoveCharacter(TextLine, "server="))
                        End If
                        If lineCount = 1 Then
                            TextBoxLocalUsername.Text = ConvertB64ToString(RemoveCharacter(TextLine, "user id="))
                        End If
                        If lineCount = 2 Then
                            TextBoxLocalPassword.Text = ConvertB64ToString(RemoveCharacter(TextLine, "password="))
                        End If
                        If lineCount = 3 Then
                            TextBoxLocalDatabase.Text = ConvertB64ToString(RemoveCharacter(TextLine, "database="))
                        End If
                        If lineCount = 4 Then
                            TextBoxLocalPort.Text = ConvertB64ToString(RemoveCharacter(TextLine, "port="))
                        End If
                        lineCount = lineCount + 1
                    Loop
                    objReader.Close()
                Else
                    ValidLocalConnection = False
                End If
            Else
                Dim path2 = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\Innovention\user.config"
                If System.IO.File.Exists(path2) Then
                    'The File exists 
                    Dim ConnStr
                    Dim ConnStr2 = ""
                    Dim CreateConnString As String = ""
                    Dim filename As String = String.Empty
                    Dim TextLine As String = ""
                    Dim objReader As New System.IO.StreamReader(path2)
                    Dim lineCount As Integer
                    Do While objReader.Peek() <> -1
                        TextLine = objReader.ReadLine()
                        If lineCount = 0 Then
                            ConnStr = ConvertB64ToString(RemoveCharacter(TextLine, "server="))
                            ConnStr2 = "server=" & ConnStr
                        End If
                        If lineCount = 1 Then
                            ConnStr = ConvertB64ToString(RemoveCharacter(TextLine, "user id="))
                            ConnStr2 += ";user id=" & ConnStr
                        End If
                        If lineCount = 2 Then
                            ConnStr = ConvertB64ToString(RemoveCharacter(TextLine, "password="))
                            ConnStr2 += ";password=" & ConnStr
                        End If
                        If lineCount = 3 Then
                            ConnStr = ConvertB64ToString(RemoveCharacter(TextLine, "database="))
                            ConnStr2 += ";database=" & ConnStr
                        End If
                        If lineCount = 4 Then
                            ConnStr = ConvertB64ToString(RemoveCharacter(TextLine, "port="))
                            ConnStr2 += ";port=" & ConnStr
                        End If
                        If lineCount = 5 Then
                            ConnStr2 += ";" & TextLine
                        End If
                        lineCount = lineCount + 1
                    Loop
                    objReader.Close()
                Else
                    ValidLocalConnection = False
                End If
            End If

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    public Sub LoadCloudConn()
        Try
            If ValidLocalConnection = True Then
                Dim Sql = "SELECT C_Server, C_Username, C_Password, C_Database, C_Port FROM loc_settings WHERE settings_id = 1"
                Dim cmd As MySqlCommand = New MySqlCommand(Sql, LocalConnection)
                Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                Dim dt As DataTable = New DataTable
                da.Fill(dt)
                If dt.Rows.Count > 0 Then
                    TextBoxCloudServer.Text = ConvertB64ToString(dt(0)(0))
                    TextBoxCloudUsername.Text = ConvertB64ToString(dt(0)(1))
                    TextBoxCloudPassword.Text = ConvertB64ToString(dt(0)(2))
                    TextBoxCloudDatabase.Text = ConvertB64ToString(dt(0)(3))
                    TextBoxCloudPort.Text = ConvertB64ToString(dt(0)(4))
                    ValidCloudConnection = True
                Else
                    ValidCloudConnection = False
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub LoadAutoBackup()
        Try
            If ValidLocalConnection = True Then
                Dim Sql = "SELECT S_BackupInterval, S_BackupDate FROM loc_settings WHERE settings_id = 1"
                Dim cmd As MySqlCommand = New MySqlCommand(Sql, LocalConnection)
                Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                Dim dt = New DataTable
                da.Fill(dt)
                If dt.Rows.Count > 0 Then
                    If dt(0)(0).ToString = "1" Then
                        RadioButtonDaily.Checked = True
                        '=================================
                        'RadioButtonWeekly.Enabled = False
                        'RadioButtonMonthly.Enabled = False
                        'RadioButtonYearly.Enabled = False
                    ElseIf dt(0)(0).ToString = "2" Then
                        RadioButtonWeekly.Checked = True
                        '=================================
                        'RadioButtonDaily.Enabled = False
                        'RadioButtonMonthly.Enabled = False
                        'RadioButtonYearly.Enabled = False
                    ElseIf dt(0)(0).ToString = "3" Then
                        RadioButtonMonthly.Checked = True
                        '=================================
                        'RadioButtonDaily.Enabled = False
                        'RadioButtonWeekly.Enabled = False
                        'RadioButtonYearly.Enabled = False
                    ElseIf dt(0)(0).ToString = "4" Then
                        RadioButtonYearly.Checked = True
                        '=================================
                        'RadioButtonDaily.Enabled = False
                        'RadioButtonWeekly.Enabled = False
                        'RadioButtonMonthly.Enabled = False
                    End If
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
#End Region
    Public Function LocalConnection() As MySqlConnection
        Dim LocalCon As MySqlConnection
        LocalCon = New MySqlConnection
        Try
            LocalCon.ConnectionString = "server=" & Trim(TextBoxLocalServer.Text) &
            ";user id=" & Trim(TextBoxLocalUsername.Text) &
            ";password=" & Trim(TextBoxLocalPassword.Text) &
            ";database=" & Trim(TextBoxLocalDatabase.Text) &
            ";port=" & Trim(TextBoxLocalPort.Text) & ";"
            LocalCon.Open()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        Return LocalCon
    End Function
    Public Function CloudConnection() As MySqlConnection
        Dim CloudCon As MySqlConnection
        CloudCon = New MySqlConnection
        Try
            CloudCon.ConnectionString = "server=" & Trim(TextBoxCloudServer.Text) &
            ";user id=" & Trim(TextBoxCloudUsername.Text) &
            ";password=" & Trim(TextBoxCloudPassword.Text) &
            ";database=" & Trim(TextBoxCloudDatabase.Text) &
            ";port=" & Trim(TextBoxCloudPort.Text) & ";"
            CloudCon.Open()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        Return CloudCon
    End Function
    Private Sub ButtonTestLocCon_Click(sender As Object, e As EventArgs) Handles ButtonTestLocCon.Click
        If LocalConnection().State = ConnectionState.Open Then
            MsgBox("Connected Succesfully")
            ValidLocalConnection = True
        Else
            MsgBox("Cannot connect to server")
            ValidLocalConnection = False
        End If
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        If CloudConnection().State = ConnectionState.Open Then
            MsgBox("Connected Succesfully")
            ValidCloudConnection = True
        Else
            MsgBox("Cannot connect to server")
            ValidCloudConnection = False
        End If
    End Sub
    Private Sub ButtonSAVELOCALCONN_Click(sender As Object, e As EventArgs) Handles ButtonSAVELOCALCONN.Click
        Try
            If ValidLocalConnection = True Then
                SaveLocalConnection()
                If TextBoxLocalUsername.ReadOnly = False Then
                    TextBoxIsReadOnly(Panel5)
                End If
            Else
                MsgBox("Local connection must be valid")
            End If

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles ButtonSAVECLOUDCONN.Click
        Try
            If ValidCloudConnection = True Then
                SaveCloudConnection()
                If TextBoxCloudUsername.ReadOnly = False Then
                    TextBoxIsReadOnly(Panel2)
                End If
            Else
                MsgBox("Cloud connection must be valid first")
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub SaveLocalConnection()
        Try
            If ValidLocalConnection = True Then
                Dim FolderName As String = "Innovention"
                Dim path = My.Computer.FileSystem.SpecialDirectories.MyDocuments
                CreateFolder(path, FolderName)
                TextBoxIsReadOnly(Panel5)
            Else
                MsgBox("Connection must be valid")
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub CreateFolder(Path As String, FolderName As String, Optional ByVal Attributes As System.IO.FileAttributes = IO.FileAttributes.Normal)
        Try
            My.Computer.FileSystem.CreateDirectory(Path & "\" & FolderName)
            If Not Attributes = IO.FileAttributes.Normal Then
                My.Computer.FileSystem.GetDirectoryInfo(Path & "\" & FolderName).Attributes = Attributes
            End If
            CreateUserConfig(Path, "user.config", FolderName)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Public Sub CreateUserConfig(path As String, FileName As String, FolderName As String, Optional ByVal Attributes As System.IO.FileAttributes = IO.FileAttributes.Normal)
        Try
            Dim CompletePath As String = path & "\" & FolderName & "\" & "user.config"
            My.Computer.FileSystem.CreateDirectory(path & "\" & FolderName)
            If Not Attributes = IO.FileAttributes.Normal Then
                My.Computer.FileSystem.GetDirectoryInfo(path & "\" & FolderName).Attributes = Attributes
            End If
            Dim ConnString(5) As String
            ConnString(0) = "server=" & ConvertToBase64(Trim(TextBoxLocalServer.Text))
            ConnString(1) = "user id=" & ConvertToBase64(Trim(TextBoxLocalUsername.Text))
            ConnString(2) = "password=" & ConvertToBase64(Trim(TextBoxLocalPassword.Text))
            ConnString(3) = "database=" & ConvertToBase64(Trim(TextBoxLocalDatabase.Text))
            ConnString(4) = "port=" & ConvertToBase64(Trim(TextBoxLocalPort.Text))
            ConnString(5) = "Allow Zero Datetime=True"
            File.WriteAllLines(CompletePath, ConnString, Encoding.UTF8)
            CreateConn(CompletePath)
            MsgBox("Saved")
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Public Sub CreateConn(path As String)
        Try
            Dim CreateConnString As String = ""
            Dim filename As String = String.Empty
            Dim TextLine As String = ""
            Dim objReader As New System.IO.StreamReader(path)
            Dim lineCount As Integer
            Do While objReader.Peek() <> -1
                TextLine = objReader.ReadLine()
                If lineCount = 0 Then
                    CreateConnString += TextLine & ";"
                End If
                If lineCount = 1 Then
                    CreateConnString += TextLine & ";"
                End If
                If lineCount = 2 Then
                    CreateConnString += TextLine & ";"
                End If
                If lineCount = 3 Then
                    CreateConnString += TextLine & ";"
                End If
                If lineCount = 4 Then
                    CreateConnString += TextLine & ";"
                End If
                If lineCount = 5 Then
                    CreateConnString += TextLine
                End If
                lineCount = lineCount + 1
            Loop
            objReader.Close()

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub SaveCloudConnection()
        Try
            Dim ConnectionLocal As MySqlConnection = LocalConnection()
            If ValidLocalConnection = True Then
                If ValidCloudConnection = True Then
                    Dim CommandHasRows As Boolean = False
                    Dim sql = "SELECT * FROM loc_settings WHERE settings_id = 1"
                    Dim cmd As MySqlCommand = New MySqlCommand(sql, ConnectionLocal)
                    Using Reader As MySqlDataReader = cmd.ExecuteReader
                        While Reader.Read
                            If Reader.HasRows Then
                                CommandHasRows = True
                            Else
                                CommandHasRows = False
                            End If
                        End While
                    End Using
                    If CommandHasRows = True Then
                        sql = "UPDATE loc_settings SET `C_Server` = @1, `C_Username` = @2, `C_Password` = @3, `C_Database` = @4, `C_Port` = @5 WHERE settings_id = 1"
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.Parameters.Add("@1", MySqlDbType.Text).Value = ConvertToBase64(Trim(TextBoxCloudServer.Text))
                        cmd.Parameters.Add("@2", MySqlDbType.Text).Value = ConvertToBase64(Trim(TextBoxCloudUsername.Text))
                        cmd.Parameters.Add("@3", MySqlDbType.Text).Value = ConvertToBase64(Trim(TextBoxCloudPassword.Text))
                        cmd.Parameters.Add("@4", MySqlDbType.Text).Value = ConvertToBase64(Trim(TextBoxCloudDatabase.Text))
                        cmd.Parameters.Add("@5", MySqlDbType.Text).Value = ConvertToBase64(Trim(TextBoxCloudPort.Text))
                        cmd.ExecuteNonQuery()
                        MsgBox("Success")
                    Else
                        sql = "INSERT INTO loc_settings (`C_Server`, `C_Username`, `C_Password`, `C_Database`, `C_Port`) VALUES (@1, @2, @3, @4, @5)"
                        cmd = New MySqlCommand(sql, ConnectionLocal)
                        cmd.Parameters.Add("@1", MySqlDbType.Text).Value = ConvertToBase64(Trim(TextBoxCloudServer.Text))
                        cmd.Parameters.Add("@2", MySqlDbType.Text).Value = ConvertToBase64(Trim(TextBoxCloudUsername.Text))
                        cmd.Parameters.Add("@3", MySqlDbType.Text).Value = ConvertToBase64(Trim(TextBoxCloudPassword.Text))
                        cmd.Parameters.Add("@4", MySqlDbType.Text).Value = ConvertToBase64(Trim(TextBoxCloudDatabase.Text))
                        cmd.Parameters.Add("@5", MySqlDbType.Text).Value = ConvertToBase64(Trim(TextBoxCloudPort.Text))
                        cmd.ExecuteNonQuery()
                        MsgBox("Success")
                    End If
                Else
                    MsgBox("Cloud connection must be valid first")
                End If
            Else
                MsgBox("Local connection must be valid first")
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub ButtonEDITLOCALCONN_Click(sender As Object, e As EventArgs) Handles ButtonEDITLOCALCONN.Click
        Try
            If ButtonEDITLOCALCONN.Text = "Edit" Then
                ButtonEDITLOCALCONN.Text = "Cancel"
            Else
                ButtonEDITLOCALCONN.Text = "Edit"
            End If
            TextBoxIsReadOnly(Panel5)
            ValidLocalConnection = False
            IFLoadedLocalCon = False
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub ButtonEDITCLOUDCONN_Click(sender As Object, e As EventArgs) Handles ButtonEDITCLOUDCONN.Click
        Try
            If ButtonEDITCLOUDCONN.Text = "Edit" Then
                ButtonEDITCLOUDCONN.Text = "Cancel"
            Else
                ButtonEDITCLOUDCONN.Text = "Edit"
            End If
            TextBoxIsReadOnly(Panel2)
            ValidCloudConnection = False
            IfLoadedCloudCon = False
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub TextBoxLocalPort_TextChanged(sender As Object, e As EventArgs) Handles TextBoxLocalPort.KeyPress, TextBoxLocalUsername.TextChanged, TextBoxLocalPassword.TextChanged, TextBoxLocalDatabase.TextChanged, TextBoxLocalServer.TextChanged
        Try
            If IFLoadedLocalCon = False Then
                ValidLocalConnection = False
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub TextBoxCloudPort_TextChanged(sender As Object, e As EventArgs) Handles TextBoxCloudPort.TextChanged, TextBoxCloudUsername.TextChanged, TextBoxCloudPassword.TextChanged, TextBoxCloudDatabase.TextChanged, TextBoxCloudServer.TextChanged
        Try
            If IfLoadedCloudCon = False Then
                ValidCloudConnection = False
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub RadioButtonDaily_Click(sender As Object, e As EventArgs) Handles RadioButtonYearly.Click, RadioButtonWeekly.Click, RadioButtonMonthly.Click, RadioButtonDaily.Click
        Try
            If ValidLocalConnection = True Then
                Dim Interval As Integer = 0
                Dim IntervalName As String = ""
                If RadioButtonDaily.Checked = True Then
                    Interval = 1
                    IntervalName = "Daily"
                ElseIf RadioButtonWeekly.Checked = True Then
                    Interval = 2
                    IntervalName = "Weekly"
                ElseIf RadioButtonMonthly.Checked = True Then
                    Interval = 3
                    IntervalName = "Monthly"
                ElseIf RadioButtonYearly.Checked = True Then
                    Interval = 4
                    IntervalName = "Yearly"
                End If
                Dim sql = "SELECT `S_BackupInterval` , `S_BackupDate` FROM loc_settings WHERE settings_id = 1"
                Dim cmd As MySqlCommand = New MySqlCommand(sql, LocalConnection)
                Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                Dim dt As DataTable = New DataTable
                da.Fill(dt)
                If dt.Rows.Count > 0 Then
                    sql = "UPDATE loc_settings SET `S_BackupInterval` = " & Interval & " , `S_BackupDate` = '" & Format(Now(), "yyyy-MM-dd") & "'"
                    cmd = New MySqlCommand(sql, LocalConnection)
                    cmd.ExecuteNonQuery()
                    Autobackup = True
                Else
                    sql = "INSERT INTO loc_settings (`S_BackupInterval` , `S_BackupDate`) VALUES ('" & Interval & "','" & Format(Now(), "yyyy-MM-dd") & "')"
                    cmd = New MySqlCommand(sql, LocalConnection)
                    cmd.ExecuteNonQuery()
                    Autobackup = True
                End If
                MsgBox("Automatic system backup set to " & IntervalName & " backup")
            Else
                Autobackup = False
                MsgBox("Local connection must be valid first.")
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub RepairDatabase()
        Try
            Process.Start("cmd.exe", "/k cd C:\xampp\mysql\bin & mysqlcheck -h " & TextBoxLocalServer.Text & " -u " & TextBoxLocalUsername.Text & " -p " & TextBoxLocalPassword.Text & " --auto-repair -c --databases " & TextBoxLocalDatabase.Text)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub OptimizeDatabase()
        Try
            Process.Start("cmd.exe", "/k cd C:\xampp\mysql\bin & mysqlcheck -h " & TextBoxLocalServer.Text & " -u " & TextBoxLocalUsername.Text & " -p " & TextBoxLocalPassword.Text & " -o --databases " & TextBoxLocalDatabase.Text)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub ButtonImport_Click(sender As Object, e As EventArgs) Handles ButtonImport.Click
        Try
            If (OpenFileDialog1.ShowDialog = DialogResult.OK) Then
                TextBoxLocalRestorePath.Text = OpenFileDialog1.FileName
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub OpenFileDialog1_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles OpenFileDialog1.FileOk
        Try
            TextBoxLocalRestorePath.Text = System.IO.Path.GetFullPath(OpenFileDialog1.FileName)
            RestoreDatabase()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub RestoreDatabase()
        Try
            Dim sql = "CREATE DATABASE /*!32312 IF NOT EXISTS*/ `" & TextBoxLocalDatabase.Text & "` /*!40100 DEFAULT CHARACTER SET utf8mb4 */;USE `" & TextBoxLocalDatabase.Text & "`;"
            Dim cmd As MySqlCommand = New MySqlCommand(sql, LocalConnection)
            cmd.ExecuteNonQuery()
            Process.Start("cmd.exe", "/k cd C:\xampp\mysql\bin & mysql -h " & TextBoxLocalServer.Text & " -u " & TextBoxLocalUsername.Text & " -p " & TextBoxLocalPassword.Text & " " & TextBoxLocalDatabase.Text & " < """ & TextBoxLocalRestorePath.Text & """")
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub BackupDatabase(ExportPath)
        Try
            Dim DatabaseName = "\" & TextBoxLocalDatabase.Text & Format(Now(), "yyyy-MM-dd") & ".sql"
            Process.Start("cmd.exe", "/k cd C:\xampp\mysql\bin & mysqldump --databases -h " & TextBoxLocalServer.Text & " -u " & TextBoxLocalUsername.Text & " -p " & TextBoxLocalPassword.Text & " " & TextBoxLocalDatabase.Text & " > """ & ExportPath & DatabaseName & """")
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub ButtonMaintenance_Click(sender As Object, e As EventArgs) Handles ButtonMaintenance.Click
        Try
            RepairDatabase()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub ButtonOptimizeDB_Click(sender As Object, e As EventArgs) Handles ButtonOptimizeDB.Click
        Try
            OptimizeDatabase()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Dim ExportPath As String = ""
    Private Sub ButtonExport_Click(sender As Object, e As EventArgs) Handles ButtonExport.Click
        Try
            If (FolderBrowserDialog1.ShowDialog() = DialogResult.OK) Then
                ExportPath = FolderBrowserDialog1.SelectedPath
                BackupDatabase(ExportPath)
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub DatePickerState(tf As Boolean)
        Try
            DateTimePicker1ACCRDI.Enabled = tf
            DateTimePicker2ACCRVU.Enabled = tf
            DateTimePicker4PTUDI.Enabled = tf
            DateTimePickerPTUVU.Enabled = tf
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
#Region "Additional Settings"
    Dim FillUp As Boolean = False
    Dim ConfirmDevInfoSettings As Boolean = False
    Private Sub ButtonSaveDevSettings_Click(sender As Object, e As EventArgs) Handles ButtonSaveDevSettings.Click
        FillUp = False
        SaveDevInfo()
    End Sub
    Private Sub SaveDevInfo()
        Dim table = "loc_settings"
        Dim where = "settings_id = 1"
        If TextboxIsEmpty(GroupBox11) = True Then
            If ValidLocalConnection = True Then
                Dim fields = "Dev_Company_Name, Dev_Address, Dev_Tin, Dev_Accr_No, Dev_Accr_Date_Issued, Dev_Accr_Valid_Until, Dev_PTU_No, Dev_PTU_Date_Issued, Dev_PTU_Valid_Until"
                Dim sql = "Select " & fields & " FROM " & table & " WHERE " & where
                Dim cmd As MySqlCommand = New MySqlCommand(sql, LocalConnection)
                Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                Dim dt As DataTable = New DataTable
                da.Fill(dt)
                If dt.Rows.Count > 0 Then
                    Dim fields1 = "`Dev_Company_Name`= '" & Trim(TextBoxDevname.Text) & "',
                `Dev_Address`= '" & Trim(TextBoxDevAdd.Text) & "',
                `Dev_Tin`= '" & Trim(TextBoxDevTIN.Text) & "',
                `Dev_Accr_No`= '" & Trim(TextBoxDevAccr.Text) & "' ,
                `Dev_Accr_Date_Issued`= '" & Format(DateTimePicker1ACCRDI.Value, "yyy-MM-dd") & "',
                `Dev_Accr_Valid_Until`= '" & Format(DateTimePicker2ACCRVU.Value, "yyyy-MM-dd") & "',
                `Dev_PTU_No`= '" & Trim(TextBoxDEVPTU.Text) & "',
                `Dev_PTU_Date_Issued`= '" & Format(DateTimePickerPTUVU.Value, "yyyy-MM-dd") & "',
                `Dev_PTU_Valid_Until`= '" & Format(DateTimePicker4PTUDI.Value, "yyyy-MM-dd") & "'"
                    sql = "UPDATE " & table & " SET " & fields1 & " WHERE " & where
                    cmd = New MySqlCommand(sql, LocalConnection)
                    cmd.ExecuteNonQuery()
                    ConfirmDevInfoSettings = True
                    If FillUp = True Then
                    Else
                        MsgBox("Saved!")
                    End If

                Else
                    Dim fields2 = "(Dev_Company_Name, Dev_Address, Dev_Tin, Dev_Accr_No, Dev_Accr_Date_Issued, Dev_Accr_Valid_Until, Dev_PTU_No, Dev_PTU_Date_Issued, Dev_PTU_Valid_Until)"
                    Dim value = "('" & Trim(TextBoxDevname.Text) & "'
                ,'" & Trim(TextBoxDevAdd.Text) & "'
                ,'" & Trim(TextBoxDevTIN.Text) & "'
                ,'" & Trim(TextBoxDevAccr.Text) & "'
                ,'" & Format(DateTimePicker1ACCRDI.Value, "yyyy-MM-dd") & "'
                ,'" & Format(DateTimePicker2ACCRVU.Value, "yyyy-MM-dd") & "'
                ,'" & Trim(TextBoxDEVPTU.Text) & "'
                ,'" & Format(DateTimePickerPTUVU.Value, "yyyy-MM-dd") & "'
                ,'" & Format(DateTimePicker4PTUDI.Value, "yyyy-MM-dd") & "')"
                    sql = "INSERT INTO " & table & " " & fields2 & " VALUES " & value
                    cmd = New MySqlCommand(sql, LocalConnection)
                    cmd.ExecuteNonQuery()
                    ConfirmDevInfoSettings = True
                    If FillUp = True Then
                    Else
                        MsgBox("Saved!")
                    End If

                End If
                TextboxEnableability(GroupBox11, False)
                DatePickerState(False)
            Else
                MsgBox("Invalid local connection")
                ConfirmDevInfoSettings = False
            End If
        Else
            MsgBox("All fields are required")
            ConfirmDevInfoSettings = False
        End If
    End Sub

    Private Sub ButtonEditDevSet_Click(sender As Object, e As EventArgs) Handles ButtonEditDevSet.Click
        TextboxEnableability(GroupBox11, True)
        DatePickerState(True)
        FillUp = False
    End Sub

    Private Sub ButtonGetExportPath_Click(sender As Object, e As EventArgs) Handles ButtonGetExportPath.Click
        If (FolderBrowserDialog1.ShowDialog() = DialogResult.OK) Then
            TextBoxExportPath.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub ButtonSaveAddSettings_Click(sender As Object, e As EventArgs) Handles ButtonSaveAddSettings.Click
        FillUp = False
        SaveAddSettings()
    End Sub
    Private Sub RDButtons(tf As Boolean)
        Try
            RadioButtonNO.Enabled = tf
            RadioButtonYES.Enabled = tf
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub ButtonEditAddSettings_Click(sender As Object, e As EventArgs) Handles ButtonEditAddSettings.Click
        TextboxEnableability(GroupBox10, True)
        RDButtons(True)
        ButtonGetExportPath.Enabled = True
        FillUp = False
    End Sub
    Dim POSVersion As String = ""
    Dim FooterInfo As String = ""
    Dim ConfirmAdditionalSettings As Boolean = False
    Private Sub SaveAddSettings()
        Try
            Dim RButton As Integer
            Dim Tax = Val(TextBoxTax.Text) / 100
            If TextboxIsEmpty(GroupBox10) = True Then
                If ValidLocalConnection = True Then
                    Dim table = "loc_settings"
                    Dim fields = "A_Export_Path, A_Tax, A_SIFormat, A_Terminal_No, A_ZeroRated, S_Zreading, S_Batter, S_Brownie_Mix, S_Upgrade_Price_Add, S_Update_Version, S_Waffle_Bag , S_Packets , P_Footer_Info"
                    Dim where = "settings_id = 1"
                    Dim sql = "Select " & fields & " FROM " & table & " WHERE " & where
                    Dim cmd As MySqlCommand = New MySqlCommand(sql, LocalConnection())
                    Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                    Dim dt As DataTable = New DataTable
                    da.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        If RadioButtonYES.Checked = True Then
                            RButton = 1
                        ElseIf RadioButtonNO.Checked = True Then
                            RButton = 0
                        End If
                        Dim fields1 = "A_Export_Path = '" & ConvertToBase64(Trim(TextBoxExportPath.Text)) & "', A_Tax = '" & Tax & "' , A_SIFormat = '" & Trim(TextBoxSINumber.Text) & "' , A_SIBeg = '" & Trim(TextBoxSIBeg.Text) & "', A_Terminal_No = '" & Trim(TextBoxTerminalNo.Text) & "' , A_ZeroRated = '" & RButton & "', S_Zreading = '" & Format(Now(), "yyyy-MM-dd") & "' , S_Batter = '" & Trim(TextBoxBATTERID.Text) & "', S_Brownie_Mix = '" & Trim(TextBoxBROWNIEID.Text) & "', S_Upgrade_Price_Add = '" & Trim(TextBoxBROWNIEPRICE.Text) & "' , `S_Waffle_Bag` = '" & Trim(TextBoxWaffleBag.Text) & "' , `S_Packets` = '" & Trim(TextBoxSugarPackets.Text) & "' , S_Update_Version = '" & POSVersion & "', P_Footer_Info = '" & FooterInfo & "'"
                        sql = "UPDATE " & table & " SET " & fields1 & " WHERE " & where
                        cmd = New MySqlCommand(sql, LocalConnection)
                        cmd.ExecuteNonQuery()
                        If FillUp = True Then
                        Else
                            MsgBox("Saved!")
                        End If
                    Else
                        Dim fields2 = "(A_Export_Path, A_Tax, A_SIFormat, A_SIBeg A_Terminal_No, A_ZeroRated, S_Zreading, S_Batter, S_Brownie_Mix, S_Upgrade_Price_Add , S_Update_Version , S_Waffle_Bag , S_Packets, P_Footer_Info)"
                        Dim value = "('" & ConvertToBase64(Trim(TextBoxExportPath.Text)) & "'
                     ,'" & Tax & "'
                     ,'" & Trim(TextBoxSINumber.Text) & "'
                     ,'" & Trim(TextBoxSIBeg.Text) & "'
                     ,'" & Trim(TextBoxTerminalNo.Text) & "'
                     ,'" & RButton & "'
                     ,'" & Format(Now(), "yyyy-MM-dd") & "'
                     ,'" & Trim(TextBoxBATTERID.Text) & "'
                     ,'" & Trim(TextBoxBROWNIEID.Text) & "'
                     ,'" & Trim(TextBoxBROWNIEPRICE.Text) & "'
                     ,'" & POSVersion & "'
                     ,'" & Trim(TextBoxWaffleBag.Text) & "'
                     ,'" & Trim(TextBoxSugarPackets.Text) & "')
                     ,'" & FooterInfo & "')"

                        sql = "INSERT INTO " & table & " " & fields2 & " VALUES " & value
                        cmd = New MySqlCommand(sql, LocalConnection)
                        cmd.ExecuteNonQuery()
                        If FillUp = True Then
                        Else
                            MsgBox("Saved!")
                        End If
                    End If
                    ConfirmAdditionalSettings = True
                    TextboxEnableability(GroupBox10, False)
                    RDButtons(False)
                    ButtonGetExportPath.Enabled = False
                Else
                    ConfirmAdditionalSettings = False
                    MsgBox("Invalid Local Connection.")
                End If
            Else
                ConfirmAdditionalSettings = False
                MsgBox("All fields are required.")
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub LoadDefaultSettingsDev()
        Try
            If ValidCloudConnection = True And ValidLocalConnection = True Then
                Sql = "SELECT `Dev_Company_Name`, `Dev_Address`, `Dev_Tin`, `Dev_Accr_No`, `Dev_Accr_Date_Issued`, `Dev_Accr_Valid_Until`, `Dev_PTU_No`, `Dev_PTU_Date_Issued`, `Dev_PTU_Valid_Until` FROM admin_settings_org WHERE settings_id = 1"
                Dim cmd As MySqlCommand = New MySqlCommand(Sql, CloudConnection)
                Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                Dim dt As DataTable = New DataTable
                da.Fill(dt)
                If dt.Rows.Count > 0 Then
                    TextBoxDevname.Text = dt(0)(0)
                    TextBoxDevAdd.Text = dt(0)(1)
                    TextBoxDevTIN.Text = dt(0)(2)
                    TextBoxDevAccr.Text = dt(0)(3)
                    DateTimePicker1ACCRDI.Text = dt(0)(4)
                    DateTimePicker2ACCRVU.Text = dt(0)(5)
                    TextBoxDEVPTU.Text = dt(0)(6)
                    DateTimePicker4PTUDI.Text = dt(0)(7)
                    DateTimePickerPTUVU.Text = dt(0)(8)
                    ConfirmDevInfoSettings = True
                Else
                    ConfirmDevInfoSettings = False
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub LoadAdditionalSettings()
        Try
            If ValidLocalConnection = True Then
                Dim sql = "SELECT A_Export_Path, A_Tax, A_SIFormat, A_SIBeg, A_Terminal_No, A_ZeroRated FROM loc_settings WHERE settings_id = 1"
                Dim cmd As MySqlCommand = New MySqlCommand(sql, LocalConnection)
                Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                Dim dt As DataTable = New DataTable
                da.Fill(dt)
                For Each row As DataRow In dt.Rows
                    If row("A_Export_Path") <> "" Then
                        If row("A_Tax") <> "" Then
                            If row("A_SIFormat") <> "" Then
                                If row("A_Terminal_No") <> "" Then
                                    If row("A_ZeroRated") <> "" Then
                                        TextBoxExportPath.Text = ConvertB64ToString(row("A_Export_Path"))
                                        TextBoxTax.Text = Val(row("A_Tax")) * 100
                                        TextBoxSINumber.Text = row("A_SIFormat")
                                        TextBoxSIBeg.Text = row("A_SIBeg")
                                        TextBoxTerminalNo.Text = row("A_Terminal_No")
                                        If Val(row("A_ZeroRated")) = 0 Then
                                            RadioButtonNO.Checked = True
                                        ElseIf dt(0)(4) = 1 Then
                                            RadioButtonYES.Checked = True
                                        End If
                                        ConfirmAdditionalSettings = True
                                    Else
                                        ConfirmAdditionalSettings = False
                                        Exit For
                                    End If
                                Else
                                    ConfirmAdditionalSettings = False
                                    Exit For
                                End If
                            Else
                                ConfirmAdditionalSettings = False
                                Exit For
                            End If
                        Else
                            ConfirmAdditionalSettings = False
                            Exit For
                        End If
                    Else
                        ConfirmAdditionalSettings = False
                        Exit For
                    End If
                Next
            Else
                ConfirmAdditionalSettings = False
            End If
            My.Settings.Save()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub LoadDefaultSettingsAdd()
        Try
            If ValidCloudConnection = True And ValidLocalConnection = True Then
                If System.IO.File.Exists(LocalConnectionPath) Then
                    Dim EXPORTPATH = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\Innovention"
                    Sql = "SELECT `A_Tax`, `A_SIFormat`, `A_SIBeg`, `A_Terminal_No`, `A_ZeroRated`, `S_Batter`, `S_Brownie_Mix`, `S_Upgrade_Price_Add` , `S_Update_Version` , `S_Waffle_Bag`, `S_Packets`, `P_Footer_Info` FROM admin_settings_org WHERE settings_id = 1"
                    Dim cmd As MySqlCommand = New MySqlCommand(Sql, CloudConnection)
                    Dim da As MySqlDataAdapter = New MySqlDataAdapter(cmd)
                    Dim dt As DataTable = New DataTable
                    da.Fill(dt)
                    If dt.Rows.Count > 0 Then
                        TextBoxExportPath.Text = EXPORTPATH
                        TextBoxTax.Text = dt(0)(0)
                        TextBoxSINumber.Text = dt(0)(1)
                        TextBoxSIBeg.Text = dt(0)(2)
                        TextBoxTerminalNo.Text = dt(0)(3)
                        If dt(0)(4) = "0" Then
                            RadioButtonNO.Checked = True
                        ElseIf dt(0)(4) = "1" Then
                            RadioButtonYES.Checked = False
                        End If
                        TextBoxBATTERID.Text = dt(0)(5)
                        TextBoxBROWNIEID.Text = dt(0)(6)
                        TextBoxBROWNIEPRICE.Text = dt(0)(7)
                        My.Settings.Version = dt(0)(8)
                        My.Settings.Save()
                        POSVersion = dt(0)(8)
                        TextBoxWaffleBag.Text = dt(0)(9)
                        TextBoxSugarPackets.Text = dt(0)(10)
                        ConfirmAdditionalSettings = True
                        FooterInfo = dt(0)(11)
                    Else
                        ConfirmAdditionalSettings = False
                    End If
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            Dim array() As String = {"loc_e_journal", "loc_zread_table", "tbcountertable", "loc_audit_trail"}

            If CheckBox2.Checked Then
                TruncateTableAll("loc_e_journal")
                RichTextBox1.Text &= "Complete : " & CheckBox2.Text & " Reset" & vbCrLf

            End If
            If CheckBox3.Checked Then
                TruncateTableAll("loc_zread_table")
                RichTextBox1.Text &= "Complete : " & CheckBox3.Text & " Reset" & vbCrLf

            End If
            If CheckBox4.Checked Then
                TruncateTableAll("tbcountertable")
                RichTextBox1.Text &= "Complete : " & CheckBox4.Text & " Reset" & vbCrLf

            End If
            If CheckBox5.Checked Then
                TruncateTableAll("loc_audit_trail")
                RichTextBox1.Text &= "Complete : " & CheckBox5.Text & " Reset" & vbCrLf
            End If

            If CheckBox1.Checked Then
                RichTextBox1.Text &= "Reset All Table" & vbCrLf
                For Each value As String In array
                    TruncateTableAll(value)
                    RichTextBox1.Text &= "Complete : " & value & " Reset" & vbCrLf
                Next
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub TruncateTableAll(ToTruncate)
        Try
            Dim ConnectionLocal As MySqlConnection = LocalConnection()
            Dim Query As String = "TRUNCATE TABLE  " & ToTruncate & " ;"
            Console.WriteLine(Query)
            Dim cmd As MySqlCommand = New MySqlCommand(Query, ConnectionLocal)
            cmd.ExecuteNonQuery()
            ConnectionLocal.Close()
            cmd.Dispose()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub TruncateWebTable(ToTruncateWeb)
        Try
            Dim ConnectionLocal As MySqlConnection = CloudConnection()
            Dim Query As String = "TRUNCATE TABLE  " & ToTruncateWeb & " ;"
            Console.WriteLine(Query)
            Dim cmd As MySqlCommand = New MySqlCommand(Query, ConnectionLocal)
            cmd.ExecuteNonQuery()
            ConnectionLocal.Close()
            cmd.Dispose()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Try
            Dim array() As String = {"admin_outlets", "admin_pos_inventory", "admin_pos_zread_inventory", "admin_serialkeys", "admin_system_logs", "loc_users", "event_logs"}

            If CheckBox6.Checked Then
                'TruncateWebTable("admin_serialkeys")
                TextBox1.Text &= CheckBox6.Text & " Table reset successfully . . ." & vbCrLf
            End If
            If CheckBox7.Checked Then
                'TruncateWebTable("admin_pos_inventory")
                TextBox1.Text &= CheckBox7.Text & " Table reset successfully . . ." & vbCrLf
            End If
            If CheckBox8.Checked Then
                'TruncateWebTable("admin_pos_zread_inventory")
                TextBox1.Text &= CheckBox8.Text & " Table reset successfully . . ." & vbCrLf
            End If
            If CheckBox9.Checked Then
                'TruncateWebTable("admin_serialkeys")
                TextBox1.Text &= CheckBox9.Text & " Table reset successfully . . ." & vbCrLf
            End If
            If CheckBox10.Checked Then
                'TruncateWebTable("admin_system_logs")
                TextBox1.Text &= CheckBox10.Text & " Table reset successfully . . ." & vbCrLf
            End If
            If CheckBox11.Checked Then
                'TruncateWebTable("loc_users")
                TextBox1.Text &= CheckBox11.Text & " Table reset successfully . . ." & vbCrLf
            End If
            If CheckBox12.Checked Then
                TruncateWebTable("event_logs")
                TextBox1.Text &= CheckBox12.Text & " Table reset successfully . . ." & vbCrLf
            End If

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Try
            Dim Connectionweb As MySqlConnection = CloudConnection()
            Dim Query As String
            Connectionweb.Close()
            If ComboBox1.Text = Trim(String.Empty) Then
                MsgBox("Please check fields if answered correctly")
            ElseIf ComboBox2.Text = Trim(String.Empty) Then
                MsgBox("Please check fields if answered correctly")
            ElseIf ComboBox2.Enabled = False Then
                MsgBox("Please check fields if answered correctly")
            ElseIf ComboBox3.Enabled = False And ComboBox4.Enabled = False Then
                If MsgBox(“This will delete all data in the table related to the Store. Are you sure you want to Proceed?”, MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                    Query = "Delete from " & ComboBox1.Text & " where store_id=" & ComboBox2.Text
                End If
            ElseIf ComboBox3.Enabled = True Then
                Query = "Delete from " & ComboBox1.Text & " where store_id=" & ComboBox2.Text & " and loc_transaction_id=" & ComboBox3.Text
            ElseIf ComboBox4.Enabled = True Then
                Query = "Delete from " & ComboBox1.Text & " where store_id=" & ComboBox2.Text & " and zreading= '" & ComboBox4.Text & "'"
            End If
            MsgBox(Query)
            'Connectionweb.Open()
            'Dim mysc As New MySqlCommand(Query, Connectionweb)
            'mysc.ExecuteNonQuery()
            'Connectionweb.Close()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub loadcombobox1() 'loading the Tablenames
        Try
            Dim Connectionweb As MySqlConnection = CloudConnection()
            Dim Query As String = "show tables"

            Dim adapter As New MySqlDataAdapter(Query, Connectionweb)

            Dim table As New DataTable

            adapter.Fill(table)

            ComboBox1.DataSource = table
            ComboBox1.ValueMember = "TABLES_IN_POSREV"
            ComboBox1.DisplayMember = "TABLES_IN_POSREV"

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub loadcombobox2() 'loading the storeID
        Try
            Dim cont As String = ComboBox1.Text

            Select Case cont
                Case "admin_pos_inventory"
                    contentsinquery()
                    ComboBox2.Enabled = True
                Case "admin_pos_zread_inventory"
                    contentsinquery()
                    ComboBox2.Enabled = True
                Case "admin_system_logs"
                    Dim Connectionweb As MySqlConnection = CloudConnection()
                    Dim Query As String = "SELECT DISTINCT log_store FROM " & ComboBox1.Text
                    Dim adapter As New MySqlDataAdapter(Query, Connectionweb)
                    Dim table As New DataTable
                    adapter.Fill(table)
                    ComboBox2.DataSource = table
                    ComboBox2.ValueMember = "log_store"
                    ComboBox2.DisplayMember = "log_store"
                    ComboBox2.Enabled = True
                    ComboBox3.Enabled = False
                    ComboBox4.Enabled = False
                'store_id=log_store
                Case "loc_users"
                    contentsinquery()
                    ComboBox2.Enabled = True
                Case "admin_outlets"
                    contentsinquery()
                    ComboBox2.Enabled = True
                Case "admin_daily_transaction"
                    contentsinquery1()
                Case "admin_daily_transaction_details"
                    contentsinquery1()
                Case Else
                    ComboBox2.SelectedIndex = -1
                    ComboBox3.SelectedIndex = -1
                    ComboBox4.SelectedIndex = -1
                    ComboBox2.Enabled = False
                    ComboBox3.Enabled = False
                    ComboBox4.Enabled = False
            End Select



        Catch ex As Exception

        End Try
    End Sub

    Private Sub contentsinquery()
        ComboBox2.SelectedIndex = -1
        ComboBox3.SelectedIndex = -1
        ComboBox4.SelectedIndex = -1
        Dim Connectionweb As MySqlConnection = CloudConnection()
        Dim Query As String = "SELECT DISTINCT STORE_ID FROM " & ComboBox1.Text
        Dim adapter As New MySqlDataAdapter(Query, Connectionweb)
        Dim table As New DataTable
        adapter.Fill(table)
        ComboBox2.DataSource = table
        ComboBox2.ValueMember = "STORE_ID"
        ComboBox2.DisplayMember = "STORE_ID"
        ComboBox2.Enabled = False
        ComboBox3.Enabled = False
        ComboBox4.Enabled = False
    End Sub

    Private Sub contentsinquery1()
        ComboBox2.SelectedIndex = -1
        ComboBox3.SelectedIndex = -1
        ComboBox4.SelectedIndex = -1
        Dim Connectionweb As MySqlConnection = CloudConnection()
        Dim Query As String = "SELECT DISTINCT STORE_ID FROM " & ComboBox1.Text
        Dim adapter As New MySqlDataAdapter(Query, Connectionweb)
        Dim table As New DataTable
        adapter.Fill(table)
        ComboBox2.DataSource = table
        ComboBox2.ValueMember = "STORE_ID"
        ComboBox2.DisplayMember = "STORE_ID"
        ComboBox2.Enabled = True
        ComboBox3.Enabled = True
        ComboBox4.Enabled = False
    End Sub
    Private Sub loadcombobox3() 'loading the transactionID
        Try
            Dim Connectionweb As MySqlConnection = CloudConnection()

            If ComboBox1.Text = "admin_daily_transaction" Then
                Dim Query As String = "SELECT DISTINCT loc_transaction_id FROM " & ComboBox1.Text & " where store_id = " & ComboBox2.Text
                Dim adapter As New MySqlDataAdapter(Query, Connectionweb)
                Dim table As New DataTable
                adapter.Fill(table)

                ComboBox3.DataSource = table
                ComboBox3.ValueMember = "loc_transaction_id"
                ComboBox3.DisplayMember = "loc_transaction_id"

            ElseIf ComboBox1.Text = "admin_daily_transaction_details" Then
                Dim Query As String = "SELECT DISTINCT loc_details_id FROM " & ComboBox1.Text & " where store_id = " & ComboBox2.Text
                Dim adapter As New MySqlDataAdapter(Query, Connectionweb)
                Dim table As New DataTable
                adapter.Fill(table)

                ComboBox3.DataSource = table
                ComboBox3.ValueMember = "loc_details_id"
                ComboBox3.DisplayMember = "loc_details_id"
            Else

            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click

        Try
            If ComboBox1.Text = Nothing Then
                MsgBox("Cannot view empty table", vbInformation)
            Else
                Tables.Show()
                Dim Connectionweb As MySqlConnection = CloudConnection()
                Connectionweb.Close()
                Dim Query As String = "SELECT * FROM " & ComboBox1.Text
                Connectionweb.Open()
                Dim Search As New MySqlDataAdapter(Query, Connectionweb)
                Dim ds As DataSet = New DataSet
                Search.Fill(ds, ComboBox1.Text)
                Tables.DataGridView1.DataSource = ds.Tables(ComboBox1.Text)
                Connectionweb.Close()
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub loadcombobox4() 'loading the transactionID
        Try
            Dim Connectionweb As MySqlConnection = CloudConnection()

            If ComboBox1.Text = "admin_daily_transaction" Then
                Dim Query As String = "SELECT DISTINCT zreading FROM " & ComboBox1.Text & " where store_id = " & ComboBox2.Text
                Dim adapter As New MySqlDataAdapter(Query, Connectionweb)
                Dim table As New DataTable
                adapter.Fill(table)

                ComboBox4.DataSource = table
                ComboBox4.ValueMember = "zreading"
                ComboBox4.DisplayMember = "zreading"
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        loadcombobox1()
    End Sub
    Private Sub ComboBox1_SelectedValueChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedValueChanged
        loadcombobox2()
        loadcombobox4()
    End Sub
    Private Sub ComboBox2_SelectedValueChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedValueChanged
        loadcombobox3()
    End Sub

    Private Sub Button5_Click_1(sender As Object, e As EventArgs) Handles Button5.Click
        ComboBox1.DataSource = Nothing
        ComboBox2.DataSource = Nothing
        ComboBox3.DataSource = Nothing
        ComboBox4.DataSource = Nothing
        ComboBox1.Items.Clear()
        ComboBox2.Items.Clear()
        ComboBox3.Items.Clear()
        ComboBox4.Items.Clear()
        CheckBox13.Checked = False
        CheckBox14.Checked = True
        ComboBox3.Enabled = False
        ComboBox2.Enabled = False
    End Sub

    Private Sub ComboBox4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox4.SelectedIndexChanged
        ComboBox3.Text = ""
        ComboBox3.Enabled = False
    End Sub

    Private Sub CheckBox13_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox13.CheckedChanged
        If CheckBox13.Checked = True Then
            ComboBox3.Enabled = False
            CheckBox14.Checked = False
        Else
            ComboBox3.Enabled = True
            CheckBox14.Checked = True

        End If
    End Sub

    Private Sub CheckBox14_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox14.CheckedChanged
        If CheckBox14.Checked = True Then
            ComboBox4.Enabled = False
            CheckBox13.Checked = False

        Else
            ComboBox4.Enabled = True
            CheckBox13.Checked = True

        End If
    End Sub



#End Region

End Class