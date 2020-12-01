Module Functions
    Public Sub TextboxEnableability(ByVal root As Control, ENB As Boolean)
        Try
            For Each ctrl As Control In root.Controls
                TextboxEnableability(ctrl, ENB)
                If TypeOf ctrl Is TextBox Then
                    CType(ctrl, TextBox).Enabled = ENB
                End If
            Next ctrl
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
    End Sub

End Module
