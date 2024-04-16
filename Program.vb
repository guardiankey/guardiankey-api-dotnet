Imports System
Imports System.Text.Json
Imports System.Text.Json.Serialization

Imports GuardianKey

Module Program
    Sub Main(args As String())
        Dim gk_conf = new Dictionary(Of String, String)
        gk_conf.Add("organization_id","")
        gk_conf.Add("authgroup_id",   "")
        gk_conf.Add("key",            "")
        gk_conf.Add("iv",             "")
        gk_conf.Add("service",        "backend")
        gk_conf.Add("agentId",        "server")

        Dim gk As New GuardianKey(gk_conf)

        Dim client_ip = "164.41.38.100"
        Dim user_agent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.146 Safari/537.36"
        Dim username = "test@test.com"
        Dim useremail = "test@test.com"
        Dim login_failed = "0"
        
        Dim ret_dict = gk.check_access(client_ip,user_agent, username , useremail, login_failed )
        Console.WriteLine(JsonSerializer.Serialize(ret_dict))
        Console.WriteLine(ret_dict.Item("response"))
    End Sub
End Module
