Public Class APIException : Inherits Exception

    Public Property ErrorCode As Integer
    Public Sub New()
        MyBase.New()
    End Sub
    Public Sub New(ByVal message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(ByVal message As String, ByVal innerException As Exception)
        MyBase.New(message, innerException)
    End Sub

    Public Sub New(errorCode As Integer, message As String)
        MyBase.New($"{errorCode}: {message}")
        Me.ErrorCode = errorCode
    End Sub
End Class
