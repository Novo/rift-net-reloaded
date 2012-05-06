Imports System.IO
Imports System.Reflection
Imports System.Xml.Serialization
Imports System.Data
Imports System.Data.SQLite

Public Module Main
    Private _flagStopListen As Boolean = False

    Sub Main()
        Dim dateTimeStarted As Date = Now

        Console.BackgroundColor = System.ConsoleColor.Black
        Console.Title = String.Format("{0} v{1}", CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyTitleAttribute), False)(0), AssemblyTitleAttribute).Title, [Assembly].GetExecutingAssembly().GetName().Version)

        Console.ForegroundColor = System.ConsoleColor.Yellow
        Console.WriteLine("{0}", CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyProductAttribute), False)(0), AssemblyProductAttribute).Product)
        Console.WriteLine(CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyCopyrightAttribute), False)(0), AssemblyCopyrightAttribute).Copyright)
        Console.WriteLine()

        Console.ForegroundColor = System.ConsoleColor.Magenta
        Console.WriteLine("http://www.easy-emu.de")
        Console.WriteLine()

        Console.ForegroundColor = System.ConsoleColor.White
        Console.WriteLine(CType([Assembly].GetExecutingAssembly().GetCustomAttributes(GetType(System.Reflection.AssemblyTitleAttribute), False)(0), AssemblyTitleAttribute).Title)
        Console.Write("Version {0}", [Assembly].GetExecutingAssembly().GetName().Version)
        Console.WriteLine()

        Console.ForegroundColor = System.ConsoleColor.Gray

        Console.Write("Setting Process Priority to HIGH...")
        Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High
        Console.WriteLine(" [done]")
        Console.WriteLine()


        'Load Configuration File
        LoadConfigFile()

        InitializeDatabases()


        'Start Realm Server
        RS = New RealmServerClass
        RS.start()

        'Start MiddleMan Server
        MS = New MiddleManServerClass
        MS.Start()

        'Start World Server
        WS = New WorldServerClass
        WS.Start()


        'Intialize Word Packet Handler
        IntializeWorldPacketHandlers()

        GC.Collect()

        Console.WriteLine("")
        Console.WriteLine("Load Time: {0}", Format(DateDiff(DateInterval.Second, dateTimeStarted, Now), "0 seconds"))
        Console.WriteLine("Used Memory: {0}", Format(GC.GetTotalMemory(True), "### ### ##0 bytes"))
        Console.WriteLine("")

        'Log Test Output
        'Log.PrintDiagnosticTest()

        'Add Console Input Commands
        AddConsoleCommand()
    End Sub


    Public Sub AddConsoleCommand()
        Dim tmp As String = "", CommandList() As String, cmds() As String
        Dim cmd() As String = {}
        Dim varList As Integer

        While Not _flagStopListen
            Try
                Console.Write("Rift>")
                tmp = Console.ReadLine()
                CommandList = tmp.Split(";")

                For varList = LBound(CommandList) To UBound(CommandList) 'accept more than one command in one row, seperated with ";"
                    cmds = Split(CommandList(varList), " ", 5) 'command + 4 parameters
                    If CommandList(varList).Length > 0 Then

                        Select Case cmds(0).ToLower

                            Case "create"
                                If cmds.Length > 1 Then

                                    Select Case cmds(1).ToLower
                                        Case "account"
                                            If cmds.Length > 3 Then
                                                Console.WriteLine("Account " & cmds(2) & " created.")
                                            Else
                                                Console.WriteLine("'create' commands are: create account [username] [password]")
                                            End If

                                        Case "realm"
                                            If cmds.Length > 2 Then

                                                Dim SQLconnect As New SQLite.SQLiteConnection()
                                                Dim SQLcommand As SQLite.SQLiteCommand

                                                SQLconnect.ConnectionString = "Data Source=database\realmDB.s3db; Version=3"
                                                SQLconnect.Open()

                                                SQLcommand = SQLconnect.CreateCommand

                                                SQLcommand.CommandText = "insert into realmlist (realm_name,gm_only) VALUES ('" & cmds(2) & "','0')"
                                                SQLcommand.ExecuteNonQuery()

                                                Console.WriteLine("Realm " & cmds(2) & " created.")
                                            Else
                                                Console.WriteLine("'create' commands are: create realm [realmname]")
                                            End If

                                        Case Else
                                            Console.WriteLine("'create' commands are: create account [username] [password]")
                                            Console.WriteLine("'create' commands are: create realm [realmname]")
                                    End Select

                                Else
                                    Console.WriteLine("'create' commands are: create account [username] [password]")
                                    Console.WriteLine("'create' commands are: create realm [realmname]")
                                End If


                            Case "quit", "shutdown", "exit"
                                Console.WriteLine("Server shutting down...")
                                _flagStopListen = True
                                RS._flagStopListen = True
                                MS._flagStopListen = True
                                WS._flagStopListen = True
                                RS.DisposeRealm()
                                MS.DisposeMiddleMan()
                                WS.DisposeWorld()
                                'ToDo: kick current Socket connections

                            Case "gccollect"
                                GC.Collect()


                            Case "info"
                                Console.WriteLine("Used memory: {0}", Format(GC.GetTotalMemory(False), "### ### ##0 bytes"))


                            Case "help"
                                Console.ForegroundColor = System.ConsoleColor.Blue
                                Console.WriteLine("Server Command list:")
                                Console.ForegroundColor = System.ConsoleColor.White
                                Console.WriteLine("---------------------------------")
                                Console.WriteLine("")
                                Console.WriteLine("")
                                Console.WriteLine("'help' - Brings up the Server Command list (this).")
                                Console.WriteLine("")
                                Console.WriteLine("'info' - Brings up a context menu showing server information (such as memory used).")
                                Console.WriteLine("")
                                Console.WriteLine("'quit' or 'shutdown' or 'exit' - Shutsdown 'WorldServer'.")
                                Console.WriteLine("")
                                Console.WriteLine("'create' - Creates acount and realm.")
                                Console.WriteLine("")
                                Console.WriteLine("'gccollect' - Call Garbage Collector to free up Memory.")
                                Console.WriteLine("")


                            Case Else
                                Console.ForegroundColor = System.ConsoleColor.DarkRed
                                Console.WriteLine("Error! Cannot find specified command. Please type 'help' for information.")
                                Console.ForegroundColor = System.ConsoleColor.Gray
                        End Select


                    End If 'of CommandList(varList).Length > 0 Then
                Next

            Catch ex As Exception
                Console.WriteLine("Error executing command [{0}]. {2}{1}", Format(TimeOfDay, "hh:mm:ss"), tmp, ex.ToString, vbNewLine)
            End Try
        End While
    End Sub



#Region "Global.Database"

    Public Sub InitializeDatabases()
        Try

            If Not File.Exists("database\realmDB.s3db") Then
                Console.Write("[{0}] Default Realm Database does not exist, creating", Format(TimeOfDay, "hh:mm:ss"))
            Else
                Console.Write("[{0}] Default Realm Database found, checking default tables", Format(TimeOfDay, "hh:mm:ss"))
            End If


            Dim SQLconnect_realmDB As New SQLite.SQLiteConnection()
            Dim SQLcommand_realmDB As SQLite.SQLiteCommand


            SQLconnect_realmDB.ConnectionString = "Data Source=database\realmDB.s3db; Version=3"
            SQLconnect_realmDB.Open()

            SQLcommand_realmDB = SQLconnect_realmDB.CreateCommand

            SQLcommand_realmDB.CommandText = "CREATE TABLE IF NOT EXISTS accounts (account_id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, username VARCHAR(32) NOT NULL, password VARCHAR(32) NOT NULL, gmlevel INTEGER NOT NULL, joindate TIMESTAMP DEFAULT CURRENT_TIMESTAMP NULL, last_ip VARCHAR(15) NULL, email TEXT NULL, banned BOOLEAN NOT NULL);"
            SQLcommand_realmDB.ExecuteNonQuery()

            Console.Write(".")

            'SQLcommand_realmDB.CommandText = "insert into accounts (Fname,Uname,PWord) VALUES ('Admin','Admin','admin')"
            'SQLcommand_realmDB.ExecuteNonQuery()

            SQLcommand_realmDB.CommandText = "CREATE TABLE IF NOT EXISTS realmlist (realm_id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, realm_name VARCHAR(32) NOT NULL, gm_only BOOLEAN NOT NULL);"
            SQLcommand_realmDB.ExecuteNonQuery()

            Console.Write(".")

            'SQLcommand_realmDB.CommandText = "insert into realmlist (realm_name,gm_only) VALUES ('Alpha Test Realm','0')"
            'SQLcommand_realmDB.ExecuteNonQuery()

            SQLcommand_realmDB.Dispose()
            SQLconnect_realmDB.Close()

            Console.WriteLine(". [done]")



            'SQLconnect_realmDB.ConnectionString = "Data Source=database\realmDB.s3db; Version=3"
            'SQLconnect_realmDB.Open()
            'SQLcommand_realmDB = SQLconnect_realmDB.CreateCommand
            'SQLcommand_realmDB.CommandText = "SELECT realm_name FROM realmlist where realm_id='1'"

            'Dim SQLreader_realmDB As SQLiteDataReader = SQLcommand_realmDB.ExecuteReader()

            'While SQLreader_realmDB.Read()
            '    If SQLreader_realmDB(0).ToString <> "" Then Config.RealmName = SQLreader_realmDB(0).ToString
            '    Console.Write(String.Format("realm_name= {0}", SQLreader_realmDB(0)))
            '    'Console.Write(String.Format("realm_id= {0}, realm_name= {1}, gm_only= {2}", SQLreader_realmDB(0), SQLreader_realmDB(1), SQLreader_realmDB(2)))
            '    Console.WriteLine("")
            'End While

            'SQLcommand_realmDB.Dispose()
            'SQLconnect_realmDB.Close()






            If Not File.Exists("database\characterDB.s3db") Then
                Console.Write("[{0}] Default Character Database does not exist, creating", Format(TimeOfDay, "hh:mm:ss"))
            Else
                Console.Write("[{0}] Default Character Database found, checking default tables", Format(TimeOfDay, "hh:mm:ss"))
            End If


            Dim SQLconnect_characterDB As New SQLite.SQLiteConnection()
            Dim SQLcommand_characterDB As SQLite.SQLiteCommand

            SQLconnect_characterDB.ConnectionString = "Data Source=database\characterDB.s3db; Version=3"
            SQLconnect_characterDB.Open()

            SQLcommand_characterDB = SQLconnect_characterDB.CreateCommand

            SQLcommand_characterDB.CommandText = "CREATE TABLE IF NOT EXISTS characters " & _
            "(guid INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, account_id INTEGER NOT NULL, name VARCHAR(32) NOT NULL, race INTEGER NOT NULL, class INTEGER NOT NULL, gender INTEGER NOT NULL, skin INTEGER NOT NULL, face INTEGER NOT NULL, hairstyle INTEGER NOT NULL, haircolor INTEGER NOT NULL, facialhair INTEGER NOT NULL, level INTEGER NOT NULL DEFAULT 1, zone INTEGER NOT NULL DEFAULT 0, map INTEGER NOT NULL DEFAULT 0, x REAL NOT NULL DEFAULT 0, y REAL NOT NULL DEFAULT 0, z REAL NOT NULL DEFAULT 0, guildId INTEGER NOT NULL DEFAULT 0, petdisplayId INTEGER NOT NULL DEFAULT 0, petlevel INTEGER NOT NULL DEFAULT 0, petfamily INTEGER NOT NULL DEFAULT 0);"
            SQLcommand_characterDB.ExecuteNonQuery()

            Console.Write(".")

            'SQLcommand_characterDB.CommandText = "insert into accounts (Fname,Uname,PWord) VALUES ('Admin','Admin','admin')"
            'SQLcommand_characterDB.ExecuteNonQuery()

            'SQLcommand_characterDB.CommandText = "CREATE TABLE IF NOT EXISTS ...);"
            'SQLcommand_characterDB.ExecuteNonQuery()

            Console.Write(".")

            'SQLcommand_characterDB.CommandText = "insert into realmlist (realm_name,gm_only) VALUES ('Alpha Test Realm','0')"
            'SQLcommand_characterDB.ExecuteNonQuery()

            SQLcommand_characterDB.Dispose()
            SQLconnect_characterDB.Close()

            Console.WriteLine(". [done]")


        Catch e As Exception
            Console.WriteLine(e.ToString)
        End Try
    End Sub

#End Region


#Region "Global.Config"
    Public Config As XMLConfigFile

    Public Class XMLConfigFile
        <XmlElement(ElementName:="LogType")> Public LogType As String = "COLORCONSOLE"
        <XmlElement(ElementName:="LogLevel")> Public LogLevel As BaseWriter.LogType = BaseWriter.LogType.FAILED
        <XmlElement(ElementName:="LogConfig")> Public LogConfig As String = "test.log"

        <XmlElement(ElementName:="RSPort")> Public RSPort As Int32 = 9100
        <XmlElement(ElementName:="RSHost")> Public RSHost As String = "127.0.0.1"

        <XmlElement(ElementName:="MMPort")> Public MMPort As Int32 = 9090
        <XmlElement(ElementName:="MMHost")> Public MMHost As String = "127.0.0.1"

        <XmlElement(ElementName:="WSPort")> Public WSPort As Int32 = 8086
        <XmlElement(ElementName:="WSHost")> Public WSHost As String = "127.0.0.1"

        <XmlElement(ElementName:="ServerLimit")> Public ServerLimit As Integer = 1

        <XmlElement(ElementName:="RealmName")> Public RealmName As String = "Alpha Test Realm"

        <XmlElement(ElementName:="AutoCreate")> Public AutoCreate As Boolean = False
    End Class

    Public Sub LoadConfigFile()
        Try
            Dim XMLFilePath As String = "config\WorldServer.xml"

            'Get filename from console arguments
            Dim args As String() = Environment.GetCommandLineArgs()
            For Each arg As String In args
                If arg.IndexOf("config") <> -1 Then
                    XMLFilePath = Trim(arg.Substring(arg.IndexOf("=") + 1))
                End If
            Next

            'Make sure a config file exists
            If System.IO.File.Exists(XMLFilePath) = False Then
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("[{0}] Error: {1} does not exist.", Format(TimeOfDay, "HH:mm:ss"), XMLFilePath)
                Console.WriteLine("Please copy the xml files into the same directory as the Server exe files.")
                Console.WriteLine("Press any key to exit server: ")
                Console.ReadKey()
                End
            End If

            'Load config
            Console.Write("[{0}] Loading Configuration from {1}", Format(TimeOfDay, "HH:mm:ss"), XMLFilePath)

            Config = New XMLConfigFile
            Console.Write(".")

            Dim oXS As XmlSerializer = New XmlSerializer(GetType(XMLConfigFile)) ' module muss public sein

            Console.Write(".")
            Dim oStmR As StreamReader
            oStmR = New StreamReader(XMLFilePath)
            Config = oXS.Deserialize(oStmR)
            oStmR.Close()

            Console.WriteLine(". [done]")


            'Creating logger
            BaseWriter.CreateLog(Config.LogType, Config.LogConfig, Log)
            Log.LogLevel = Config.LogLevel

        Catch e As Exception
            Console.WriteLine(e.ToString)
        End Try
    End Sub

#End Region


End Module
