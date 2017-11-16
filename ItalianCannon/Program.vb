Module Program
    Sub Main(args As String())
        'Arguments specified
        If Not args.Count = 0 Then
            ResolveArguments(args)
            'Read resolved arguments
            If Constants.CurrentCommandLine.GenConf Then
                Configurations.Initiate()
                Environment.Exit(0)
            End If
            If Constants.CurrentCommandLine.DisplayHelp Then
                Console.WriteLine(Constants.CommandLineHelp)
                Environment.Exit(0)
            End If
			If Constants.CurrentCommandLine.AnimationsEnabled Then
				Dim thrAnimations As New Threading.Thread(AddressOf ThreadAnimations)
				thrAnimations.Start()
			End If
        End If

        'Proceed other stuff.
        If Not Constants.CurrentCommandLine.VerboseMode Then Console.WriteLine(Constants.ASCIIART)
        Out("ItalianCannon version " & Constants.AppVer, , , False)
        Out("Initiating color profiles...", , , False)
        dpreColor = Console.ForegroundColor
        dbakColor = Console.BackgroundColor
        Configurations.Initiate()
        'Apply -c.
        If Constants.CurrentCommandLine.NoSingleThrLimit Then
            Constants.CurrentConfigurations.MaxRequestsPerThread = 0
        End If
        Out("Starting threads... Your current threads number was " & Constants.CurrentConfigurations.Threads & ".")
        Constants.SW.Start()
        For i = 1 To Constants.CurrentConfigurations.Threads
            Dim thread As New Threading.Thread(AddressOf ThreadRun)
            thread.Start()
        Next
        Dim input As String = ""
        Out("Started. Type 'exit' to stop.")
Read:
        input = Console.ReadLine().ToLower()
        If input = "exit" Then Environment.Exit(0)
        Out("You can type 'exit' or press ^C to stop.", , LogLevels.WARN)
        GoTo Read
    End Sub

    Sub ThreadRun()
        Constants.ThrId += 1
        Dim ThrId As Integer = Constants.ThrId
        Out("Starting...", "THR" & ThrId)
        Dim i As Long = 1
        Do Until i = Constants.CurrentConfigurations.MaxRequestsPerThread
            Try
                Dim client As New Net.WebClient()
                client.Headers.Add(Net.HttpRequestHeader.UserAgent, Constants.CurrentConfigurations.UserAgent)
                client.DownloadData(Constants.CurrentConfigurations.TeaCupTarget)
                client.Dispose()
                Constants.Total += 1
                Out("REQ OK", Constants.SW.Elapsed.ToString & "/" & i & "thr./" & Constants.Total & "ts" & "/THR" & ThrId)
            Catch ex As Exception
				Constants.TotalFail += 1
                Out("REQ ERR: " & ex.Message, Constants.SW.Elapsed.ToString & "/" & i & "thr./" & Constants.Total & "ts" & "/THR" & ThrId, LogLevels.EXCEPTION)
            End Try
            Threading.Thread.Sleep(Constants.CurrentConfigurations.IntervalPerThread)
            i += 1
        Loop
        Out("Max requests limit exceeded. Stopped.", Constants.SW.Elapsed.ToString & "/" & Constants.CurrentConfigurations.MaxRequestsPerThread & "thr./" & Constants.Total & "ts" & "/THR" & ThrId)
    End Sub
	
	Sub ThreadAnimations()
		'[###       ][00:00:00.000000][500THR][100TS/1FL][MAX0]
		Dim count As UInt64 = 0
		Dim prevLength As Short = 0
		Console.Write("Starting...")
		Threading.Thread.Sleep(1000)
		For i = 1 To 11 Step 1
			Console.Write(vbBack)
		Next
		Do Until 233 = 2333
			If Not prevLength = 0 Then 'If not first time, check and delete.
				For i = 0 To prevLength
					Console.Write(vbBack)
				Next
			End If
			Dim str As String = ""
			str &= GetAnimationChar(count Mod 13)
			str &= "[" & Constants.SW.Elapsed.ToString & "]"
			str &= "[" & Constants.ThrId & "/" & Constants.CurrentConfigurations.Threads & "THR]"
			str &= "[" & Constants.Total & "TS/" & Constants.TotalFail & "FL]"
			str &= "[MAX" & Constants.CurrentConfigurations.MaxRequestsPerThread & "]"
			Console.Write(str)
			prevLength = str.Length 'Set length for next deletion.
			count += 1
			Threading.Thread.Sleep(250)
		Loop
	End Sub

	''' <summary>
	''' Get animation block char, from 0 to 12.
	''' Case Else, equals to 12.
	''' </summary>
	''' <returns></returns>
	Function GetAnimationChar(percentage As Integer) As String
		Select Case percentage
			Case 0
				Return "[#         ]"
			Case 1
				Return "[##        ]"
			Case 2
				Return "[###       ]"
			Case 3
				Return "[ ###      ]"
			Case 4
				Return "[  ###     ]"
			Case 5
				Return "[   ###    ]"
			Case 6
				Return "[    ###   ]"
			Case 7
				Return "[     ###  ]"
			Case 8
				Return "[      ### ]"
			Case 9
				Return "[       ###]"
			Case 10
				Return "[        ##]"
			Case 11
				Return "[         #]"
			Case Else
				Return "[          ]"
		End Select
	End Function
	
    Sub ResolveArguments(args As String())
        Dim fullArgs As String = ""
        For Each s As String In args
            fullArgs &= s & " "
        Next
        If fullArgs.ToLower().Contains("-v") Then
            Constants.CurrentCommandLine.VerboseMode = True
        Else
            Constants.CurrentCommandLine.VerboseMode = False
        End If
        If fullArgs.ToLower().Contains("-c") Then
            Constants.CurrentCommandLine.NoSingleThrLimit = True
        Else
            Constants.CurrentCommandLine.NoSingleThrLimit = False
        End If
        If fullArgs.ToLower().Contains("--genconf") Then
            Constants.CurrentCommandLine.GenConf = True
        Else
            Constants.CurrentCommandLine.GenConf = False
        End If
        If fullArgs.ToLower().Contains("--help") Then
            Constants.CurrentCommandLine.DisplayHelp = True
        Else
            Constants.CurrentCommandLine.DisplayHelp = False
        End If
        If fullArgs.ToLower().Contains("-g") Then
            Constants.CurrentCommandLine.DisableColor = True
        Else
            Constants.CurrentCommandLine.DisableColor = False
        End If
		If fullArgs.ToLower().Contains("-a") Then
			Constants.CurrentCommandLine.AnimationsEnabled = True
		Else
			Constants.CurrentCommandLine.AnimationsEnabled = False
		End If
    End Sub
End Module
