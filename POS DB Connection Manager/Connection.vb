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
    Private Sub Connection_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            TabControl2.TabPages(0).Text = "Connection Settings"
            LoadConn()
            LoadCloudConn()
            LoadAutoBackup()
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
    Private Sub LoadCloudConn()
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
End Class