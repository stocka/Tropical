Class Application

  Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup

    Dim mainWin As New MainWindow()
    mainWin.DataContext = New SpriteSheetViewModel(New Models.SpriteSheet(), mainWin)
    Me.MainWindow = mainWin
    Me.MainWindow.Visibility = Visibility.Visible

  End Sub

End Class
