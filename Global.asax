<%@ Application Language="VB" %>
<script RunAt="server">

    Dim LogFile As SCFramework.LogFile = Nothing
    
    Private Sub WriteLog(Message As String)
        If LogFile IsNot Nothing Then
            LogFile.Add(Message)
        End If
    End Sub
    
    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Timeout
        Server.ScriptTimeout = 3600
        
        ' Check DB
        Try
            SCFramework.Bridge.Query.RecoverFromBackup()
            Me.WriteLog("DB OK")
            
        Catch ex As Exception
            LogFile.Add("Error while check DB: ", ex.Message)
        End Try
        
        ' Init
        Dim Configuration As SCFramework.SystemConfig = New SCFramework.SystemConfig
        Application("SystemConfig") = Configuration
        
        ' Logs
        LogFile = New SCFramework.LogFile(Configuration.SystemLogFilePath, False, False)
        LogFile.WaitWriteCommandBeforeSave = False
        Me.WriteLog("Application is started")
        
        ' Mailer
        If Configuration.UseBackgroundMailer Then
            Application("MailerCycle") = New SCFramework.MailsCycle(Configuration.LogsPath & "\Mailer.log")
        End If  
        
        ' Crawler  
        If Configuration.UseBackgroundCrawler Then
            Application("CrawlerCycle") = New SCFramework.Crawler("private")
        End If
        
        ' Cycles
        Application("CleanerCycle") = New SCFramework.CleanTemporaryDatasCycle()
    End Sub
    
    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Stop threads
        Application("MailerCycle") = Nothing
        Application("CrawlerCycle") = Nothing
        Application("CampaignsCycle") = Nothing
        Application("CleanerCycle") = Nothing
        Me.WriteLog("Threads is stopped")
       
        ' Copmpress DB
        Try
            SCFramework.Bridge.Query.CompactAndRepair()
            Me.WriteLog("Compact and repair DB")
            
        Catch ex As Exception
            LogFile.Add("Error while compact DB: ", ex.Message)
        End Try
        
        ' Logs
        Me.WriteLog("Application is stopped")
    End Sub
        
    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Logs
        Dim objErr As Exception = Server.GetLastError()
        If TypeOf objErr Is HttpUnhandledException Then
            Me.WriteLog("Unhandled Application error: ")
        Else
            Me.WriteLog("Application error: " & objErr.Message)
        End If
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        Try
            ' Registrazione utenti connessi
            SCFramework.Bridge.Configuration.GlobalCounter += 1
            SCFramework.Stats.Trace()
            
        Catch ex As Exception
        End Try
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
    End Sub
    
</script>
