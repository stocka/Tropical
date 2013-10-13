Class Application

  Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup

    ' We're doing all of this so that we can start up with a fresh
    ' DataContext, which also means not using a StartupUri.
    Dim mainWin As New MainWindow()
    mainWin.ChangeCurrentSpriteSheet(New Models.SpriteSheet())

    Me.MainWindow = mainWin

    ' This is needed because we're not using the StartupUri property.
    Me.MainWindow.Visibility = Visibility.Visible

  End Sub

End Class
