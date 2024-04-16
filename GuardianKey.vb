Imports System
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports System.Net 
Imports System.Net.Http
Imports System.Text

Public Class GuardianKeyEvent
    Public Property generatedTime     As String
    Public Property agentId           As String
    Public Property organizationId    As String
    Public Property authGroupId       As String
    Public Property service           As String
    Public Property clientIP          As String
    Public Property clientReverse     As String
    Public Property userName          As String
    Public Property authMethod        As String
    Public Property loginFailed       As String
    Public Property userAgent         As String
    Public Property psychometricTyped As String
    Public Property psychometricImage As String
    Public Property event_type        As String
    Public Property userEmail         As String
End Class

Public Class GuardianKey
    Private Property organization_id As String
    Private Property authgroup_id    As String
    Private Property key     As String
    Private Property iv      As String
    Private Property service As String
    Private Property agentId As String
    Private Property api_url As String
    Private Property api_url_gktinc As String

    Public Sub New(gk_conf as Dictionary(Of String, String))
      organization_id = gk_conf.Item("organization_id")
      authgroup_id    = gk_conf.Item("authgroup_id")
      key             = gk_conf.Item("key")
      iv              = gk_conf.Item("iv")
      service         = gk_conf.Item("service")
      agentId         = gk_conf.Item("agentId")
      api_url         = "https://api.guardiankey.io/v2/checkaccess"
      api_url_gktinc  = "https://api.guardiankey.io/v2/checkgktinc"
    End Sub

  Public Function create_event(client_ip As String,user_agent  As String, username  As String, useremail  As String, login_failed  As String)
    Dim gkEvent As New GuardianKeyEvent()
    With gkEvent
         .generatedTime   = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
         .agentId         = agentId
         .organizationId  = organization_id
         .authGroupId     = authgroup_id
         .service         = service
         .clientIP        = client_ip
         .clientReverse   = ""
         .userName        = username
         .authMethod      = ""
         .loginFailed     = login_failed
         .userAgent       = user_agent
         .psychometricTyped = ""
         .psychometricImage = ""
         .event_type      = "Authentication"
         .userEmail       = useremail
    End With
    Return JsonSerializer.Serialize(gkEvent)
  End Function

  Public Function SHA256Hash(my_string As String)
      Dim crypt As New System.Security.Cryptography.SHA256CryptoServiceProvider
      Dim ByteString() As Byte = System.Text.Encoding.ASCII.GetBytes(my_string)
      Dim result As String = Nothing
      ByteString = crypt.ComputeHash(ByteString)
      For Each bt As Byte In ByteString
          result &= bt.ToString("x2")
      Next
      Return result
  End Function

  Public Function check_access(client_ip As String, user_agent As String, username As String, useremail As String, login_failed As String)
    dim event_str :  event_str     = create_event(client_ip,user_agent,username,useremail,login_failed)
    dim hash      :  hash =  SHA256Hash(event_str & key &  iv )
    Dim msg_dict = new Dictionary(Of String, String)
    msg_dict.Add("id",      authgroup_id)
    msg_dict.Add("message", event_str)
    msg_dict.Add("hash",    hash)
    dim payload As String = JsonSerializer.Serialize(msg_dict)
    Return post_payload(payload,api_url)
  End Function

  Private Function post_payload(payload As String, api_url_here As String)
    Try
      Dim Uri As New Uri(String.Format(api_url_here))
      Dim webClient As New WebClient()
      Dim resByte As Byte()
      Dim resString As String
      Dim reqString() As Byte
      webClient.Headers("content-type") = "application/json"
      reqString = Encoding.Default.GetBytes(payload)
      resByte = webClient.UploadData(Uri, "post", reqString)
      resString = Encoding.Default.GetString(resByte)
      Dim return_dict As Dictionary(Of String, String) = JsonSerializer.Deserialize(Of Dictionary(Of String, String))(resString)
      Return return_dict
    Catch ex As Exception
      Dim return_dict As Dictionary(Of String, String) = JsonSerializer.Deserialize(Of Dictionary(Of String, String))("{""response"":""ERROR""}")
      Return return_dict
    End Try
  End Function

End Class