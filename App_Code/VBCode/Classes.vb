Imports System.Drawing
Imports System.Web.Hosting

' --------------------------------------------------------------------------------------
' Common utility
' --------------------------------------------------------------------------------------
Public Class Common

    ' Add an alert message at the script manager
    Public Shared Sub AddAlertMessage([Control] As Object, Message As String)
        If TypeOf Control Is Telerik.Web.UI.RadAjaxManager Or TypeOf Control Is Telerik.Web.UI.RadAjaxPanel Then
            Message = Message.Replace("'", "")
            Dim Script As String = "alert('" & Message & "');"
            [Control].ResponseScripts.Add(Script)
        End If
    End Sub

End Class


' --------------------------------------------------------------------------------------
' Internal styles
' --------------------------------------------------------------------------------------
Public Class InternalStyles

    ' Default grid style
    Public Shared TelerikGridStyle As String = "MetroTouch"

End Class


' --------------------------------------------------------------------------------------
' Encorder
' --------------------------------------------------------------------------------------
Public Class Encoder

    ' Define the characters map to use for encode/decode numbers
    Private Shared Map As Char() = { _
        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", _
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "m", "n", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" _
    }

    ' Encode number to have a compressed string rappresentation
    Public Shared Function EncodeNumberToString(Value As Long) As String
        ' Define the holder
        Dim Result As String = String.Empty

        ' Only positive numbers
        If Value > 0 Then
            ' If value is 0 return the first letter of map
            If Value = 0 Then Return Map(0)

            ' Find the map length
            Dim MapLen As Integer = Map.Length

            ' Decrement the value of the rest while value is major of zero
            While Value > 0
                ' Find the module
                Dim Rest = Value Mod MapLen
                ' Find the new value
                Value = Value / MapLen
                ' Build the string with the rest
                Result = Map(Rest) & Result
            End While

        End If
        Return Result
    End Function

    ' Decode string encoded with "EncodeNumberToString" function
    Public Shared Function DecodeStringToNumber(Value As String) As Long
        ' Check the value
        If Value = String.Empty Then Return -1

        ' Define the result holder
        Dim Result As Long = 0

        ' Find the string and the map length
        Dim MapLen As Integer = Map.Length
        Dim StrLen As Integer = Value.Length

        ' Cycle all characters of encoded string
        For Index As Integer = 0 To StrLen - 1
            ' Find the current character
            Dim Character As Char = Value(StrLen - 1 - Index)
            ' Find the character position inside Map array
            Dim Position As Integer = Array.IndexOf(Map, Character)
            ' Calc the new value
            Result += Position * Math.Pow(MapLen, Index)
        Next
        Return Result
    End Function

End Class


' --------------------------------------------------------------------------------------
' URLs
' --------------------------------------------------------------------------------------
Public Class URLs

    ' The name of table relative at database
    Public Const DATABASE_TABLENAME As String = "LST_URLS"

    ' Get the record date of creation 
    Public Shared Function GetCreationDate(ID As Long) As Date
        ' Create the SQL
        Dim SQL As String = "SELECT [CREATION] " & _
                            "FROM [" & DATABASE_TABLENAME & "] " & _
                            "WHERE [ID_URL] = " & SCFramework.DBStringAdapter.Numeric(ID)
        ' Check the result
        Dim Value As Object = SCFramework.Bridge.Query.Value(SQL)
        If Value Is Nothing Then
            ' If null return the min date
            Return Date.MinValue
        Else
            ' Return the value
            Return CDate(Value)
        End If
    End Function

    ' Get the long url from database
    Public Shared Function GetLongURL(ID As Long) As String
        ' Create the SQL
        Dim SQL As String = "SELECT [URL] " & _
                            "FROM [" & DATABASE_TABLENAME & "] " & _
                            "WHERE [ID_URL] = " & SCFramework.DBStringAdapter.Numeric(ID)
        ' Return the value
        Return "" & SCFramework.Bridge.Query.Value(SQL)
    End Function

    ' Get the datable source
    Public Shared Function GetSource() As DataTable
        ' Create SQL
        Dim SQL As String = "SELECT * " & _
                            "FROM [" & DATABASE_TABLENAME & "]"
        ' Table
        Dim Table As DataTable = SCFramework.Bridge.Query.Table(SQL, DATABASE_TABLENAME)

        ' Add extra column
        Table.Columns.Add("SHORT", GetType(System.String))

        ' Fix extra column
        For Each Row As DataRow In Table.Rows
            ' Create the code
            Dim Encoded As String = Global.Encoder.EncodeNumberToString(Row!ID_URL)

            ' Create a short URL and print at screen
            Row!SHORT = String.Format("{0}/?{1}", SCFramework.Utils.GetAppURLDomain(), Encoded)
        Next

        ' return
        Return Table
    End Function

        ' Save the URL inside database and return the ID reference
    Public Shared Function RegisterURL(URL As String) As Long
        ' Check if is valid
        If Not SCFramework.Utils.IsValidURL(URL) Then
            Throw New Exception("Insert a valid URL!")
        End If

        ' Before check if already exists
        ' Create the SQL
        Dim SQL As String = "SELECT [ID_URL] " & _
                            "FROM [" & DATABASE_TABLENAME & "] " & _
                            "WHERE [URL] = " & SCFramework.DBStringAdapter.String(URL)
        ' Retrieve the ID
        Dim Value As Object = SCFramework.Bridge.Query.Value(SQL)

        ' Check value
        If Value IsNot Nothing Then
            ' The URL is already insered so return the ID
            Return CLng(Value)
        Else
            ' The URL is not insered so do it
            SQL = "INSERT INTO [" & DATABASE_TABLENAME & "] (" & _
                    "[URL]" & _
                  ") VALUES (" & _
                    SCFramework.DBStringAdapter.String(URL) & _
                  ")"
            ' Insert the URL and return the new ID
            Return SCFramework.Bridge.Query.Exec(SQL, True)
        End If
    End Function

    ' Delete a URL row from database
    Public Shared Sub DeleteURL(ID As Long)
        ' Create the SQL
        Dim SQL As String = "DELETE FROM [" & DATABASE_TABLENAME & "] " & _
                            "WHERE [ID_URL] = " & SCFramework.DBStringAdapter.Numeric(ID)
        ' Execute
        SCFramework.Bridge.Query.Exec(SQL)
    End Sub

    ' Update URL in database
    Public Shared Sub UpdateURL(ID As Long, URL As String)
        ' Check if is valid
        If Not SCFramework.Utils.IsValidURL(URL) Then
            Throw New Exception("Insert a valid URL!")
        End If

        ' Create SQL command
        Dim SQL As String = "UPDATE [" & DATABASE_TABLENAME & "] " & _
                            "SET [URL] = " & SCFramework.DBStringAdapter.String(URL) & " " & _
                            "WHERE [ID_URL] = " & SCFramework.DBStringAdapter.Numeric(ID)
        ' Execute
        SCFramework.Bridge.Query.Exec(SQL)
    End Sub

End Class