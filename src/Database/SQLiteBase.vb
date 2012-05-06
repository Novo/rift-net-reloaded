Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Data.SQLite
Imports System.Data

Public Class SQLiteBase
    Implements IDisposable

    Private Connection As SQLiteConnection
    Private SqlData As SQLiteDataReader

    Private m_RowCount As Integer

    Public Property RowCount() As Integer
        Get
            Return m_RowCount
        End Get

        Set(ByVal value As Integer)
            m_RowCount = value
        End Set
    End Property


    Public Sub New(ByVal db As String)
        Connection = New SQLiteConnection("Data Source=Database\" & db & ".s3db; Version=3")

        Try
            Connection.Open()
        Catch ex As SQLiteException
            'Log.Message(LogType.[ERROR], "{0}", ex.Message)
        End Try
    End Sub


    Public Sub DisposeDatabaseConnection() Implements System.IDisposable.Dispose
        SqlData.Close()
        Connection.Close()
    End Sub


    Public Function Execute(ByVal sql As String, ByVal ParamArray args As Object()) As Boolean
        Dim sqlString As New StringBuilder()
        sqlString.AppendFormat(sql, args)

        Dim sqlCommand As New SQLiteCommand(sqlString.ToString(), Connection)

        Try
            sqlCommand.ExecuteNonQuery()

            Return True
        Catch ex As SQLiteException
            'Log.Message(LogType.[ERROR], "{0}", ex.Message)
            Return False
        End Try

        sqlCommand.Dispose()
        SqlData.Close()
        Connection.Close()

    End Function


    Public Function [Select](ByVal sql As String) As DataTable
        Dim retData As New DataTable()
        Dim sqlCommand As New SQLiteCommand(sql, Connection)

        Try
            SqlData = sqlCommand.ExecuteReader(CommandBehavior.[Default])
            retData.Load(SqlData)

        Catch ex As SQLiteException
            'Log.Message(LogType.[ERROR], "{0}", ex.Message)
        End Try

        sqlCommand.Dispose()
        SqlData.Close()
        Connection.Close()

        Return retData

    End Function


End Class
