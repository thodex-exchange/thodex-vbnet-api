Imports System.Net
Imports System.Security.Cryptography
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports RestSharp

Module ThodeAPI
    ReadOnly apisecret = ""
    ReadOnly apikey = ""
    ReadOnly url = "https://api.thodex.com/v1"

    Sub Main()
        Try
            Console.WriteLine(GetBalance())
            'Console.WriteLine(GetServerTime())
            'Console.WriteLine(GetMarkets())
            'Console.WriteLine(GetMarketStatus("BTCTRY"))
            'Console.WriteLine(GetMarketSummary())
            'Console.WriteLine(GetMarketHistory("BTCTRY", 12345678))
            'Console.WriteLine(GetOrderDepth("BTCTRY", 5))
            'Console.WriteLine(GetOpenOrders("BTCTRY", 5, 5))
            'Console.WriteLine(GetOrderHistory("BTCTRY", 5, 5))
            'Console.WriteLine(PostBuyLimit("BTCTRY", 32500.5, 0.01))
            'Console.WriteLine(PostSellLimit("BTCTRY", 32500.5, 0.01))
            'Console.WriteLine(PostBuyMarket("BTCTRY", 32500.5))
            'Console.WriteLine(PostSellMarket("BTCTRY", 0.01))
            'Console.WriteLine(PostCancelOrder("BTCTRY", 12345678))
        Catch ex As APIException
            Console.WriteLine("ERROR CODE[" + ex.ErrorCode.ToString + "] ERROR MSG[" + ex.Message + "]")
        End Try

        Console.ReadKey()
    End Sub

    Function GetBalance() As JObject
        Dim params As New SortedDictionary(Of String, Object)
        Dim route As String = "account/balance"
        Return MakeRequest(route, params, Method.GET)
    End Function

    Function GetServerTime() As JObject
        Dim params As New SortedDictionary(Of String, Object)
        Dim route As String = "public/time"

        Return MakeRequest(route, params, Method.GET)
    End Function

    Function GetMarkets() As JObject
        Dim params As New SortedDictionary(Of String, Object)
        Dim route As String = "public/markets"

        Return MakeRequest(route, params, Method.GET)
    End Function

    Function GetMarketStatus(market As String) As JObject
        Dim params As New SortedDictionary(Of String, Object)
        params.Add("market", market)
        Dim route As String = "public/market-status"

        Return MakeRequest(route, params, Method.GET)
    End Function

    Function GetMarketSummary() As JObject
        Dim params As New SortedDictionary(Of String, Object)
        Dim route As String = "public/market-summary"

        Return MakeRequest(route, params, Method.GET)
    End Function

    Function GetMarketHistory(market As String, Optional lastid As Integer = 0) As JObject
        Dim params As New SortedDictionary(Of String, Object)
        params.Add("market", market)
        If lastid > 0 Then
            params.Add("last_id", lastid.ToString)
        End If
        Dim route As String = "public/market-history"

        Return MakeRequest(route, params, Method.GET)
    End Function

    Function GetOrderDepth(market As String, Optional limit As Integer = 0) As JObject
        Dim params As New SortedDictionary(Of String, Object)
        params.Add("market", market)
        If limit > 0 Then
            params.Add("limit", limit)
        End If
        Dim route As String = "public/order-depth"

        Return MakeRequest(route, params, Method.GET)
    End Function

    Function GetOpenOrders(market As String, Optional offset As Integer = 0, Optional limit As Integer = 0) As JObject
        Dim params As New SortedDictionary(Of String, Object)
        params.Add("market", market)
        If offset > 0 Then
            params.Add("offset", offset)
        End If
        If limit > 0 Then
            params.Add("limit", limit)
        End If
        Dim route As String = "market/open-orders"

        Return MakeRequest(route, params, Method.GET)
    End Function

    Function GetOrderHistory(market As String, Optional offset As Integer = 0, Optional limit As Integer = 0) As JObject
        Dim params As New SortedDictionary(Of String, Object)
        params.Add("market", market)
        If offset > 0 Then
            params.Add("offset", offset)
        End If
        If limit > 0 Then
            params.Add("limit", limit)
        End If
        Dim route As String = "market/order-history"

        Return MakeRequest(route, params, Method.GET)
    End Function

    Function PostBuyLimit(market As String, price As Double, amount As Double) As JObject
        Dim params As New SortedDictionary(Of String, Object)
        params.Add("market", market)
        params.Add("price", price)
        params.Add("amount", amount)
        Dim route As String = "market/buy-limit"

        Return MakeRequest(route, params, Method.POST)
    End Function

    Function PostSellLimit(market As String, price As Double, amount As Double) As JObject
        Dim params As New SortedDictionary(Of String, Object)
        params.Add("market", market)
        params.Add("price", price)
        params.Add("amount", amount)
        Dim route As String = "market/sell-limit"

        Return MakeRequest(route, params, Method.POST)
    End Function

    Function PostBuyMarket(market As String, amount As Double) As JObject
        Dim params As New SortedDictionary(Of String, Object)
        params.Add("market", market)
        params.Add("amount", amount)
        Dim route As String = "market/buy"

        Return MakeRequest(route, params, Method.POST)
    End Function

    Function PostSellMarket(market As String, amount As Double) As JObject
        Dim params As New SortedDictionary(Of String, Object)
        params.Add("market", market)
        params.Add("amount", amount)
        Dim route As String = "market/sell"

        Return MakeRequest(route, params, Method.POST)
    End Function

    Function PostCancelOrder(market As String, orderid As Integer) As JObject
        Dim params As New SortedDictionary(Of String, Object)
        params.Add("market", market)
        params.Add("order_id", orderid)
        Dim route As String = "market/cancel"

        Return MakeRequest(route, params, Method.POST)
    End Function

    Function MakeRequest(route As String, params As SortedDictionary(Of String, Object), httpMethod As RestSharp.Method) As JObject
        Dim content As String
        Dim authorization As String
        Dim obj As JObject
        Dim resp As RestResponse
        Dim req As New RestRequest(route, httpMethod)
        Dim rc As New RestClient()
        rc.BaseUrl = New Uri(url)

        params.Add("tonce", GetUnixTimestamp())
        params.Add("apikey", apikey)

        For Each item In params
            req.AddParameter(item.Key, item.Value)
        Next
        authorization = encode(params)

        req.AddHeader("cache-control", "no-cache")
        req.AddHeader("Authorization", authorization)

        resp = rc.Execute(req)

        content = resp.Content
        rc = Nothing
        req = Nothing
        resp = Nothing
        obj = JObject.Parse(content)
        If Not obj.Item("error").Type = JTokenType.Null Then Throw New APIException(CInt(obj.Item("error")("code").ToString), obj.Item("error")("message").ToString)
        Return obj
    End Function

    Function encode(params As SortedDictionary(Of String, Object)) As String
        If params.ContainsKey("attachments") Then
            params.Remove("attachments")
        End If
        Dim hashparams As New Dictionary(Of String, Object)(params)
        hashparams.Add("secret", apisecret) ' secret parameter must be last item
        Dim query As String = BuildQueryString(hashparams)
        Dim hash As String = GenerateSHA256String(query)
        Return hash.ToLower
    End Function

    Function GetUnixTimestamp() As Integer
        Return (DateTime.Now.ToUniversalTime - New DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
    End Function

    Function BuildQueryString(ByVal parameters As Dictionary(Of String, Object)) As String
        Return String.Join("&", parameters.Select(Function(pair) $"{pair.Key}={WebUtility.UrlEncode(pair.Value)}"))
    End Function

    Function GenerateSHA256String(ByVal inputString) As String
        Dim sha256 As SHA256 = SHA256Managed.Create()
        Dim bytes As Byte() = Encoding.UTF8.GetBytes(inputString)
        Dim hash As Byte() = sha256.ComputeHash(bytes)
        Dim stringBuilder As New StringBuilder()

        For i As Integer = 0 To hash.Length - 1
            stringBuilder.Append(hash(i).ToString("X2"))
        Next

        Return stringBuilder.ToString()
    End Function

End Module
