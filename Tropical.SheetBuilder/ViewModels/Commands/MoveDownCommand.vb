Imports Tropical.Models

Public Class MoveDownCommand
  Inherits MoveCommandBase

  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 movedDown As Action(Of Sprite))
    MyBase.New(service, canExecute, movedDown)
  End Sub

  Public Overrides Sub Execute(parameter As Object)
    ' Move down.
    MyBase.ExecuteMoveBase(parameter, False)
  End Sub

End Class
