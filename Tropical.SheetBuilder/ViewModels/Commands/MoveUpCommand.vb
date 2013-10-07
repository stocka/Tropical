Imports Tropical.Models

Public Class MoveUpCommand
  Inherits MoveCommandBase

  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 movedUp As Action(Of Sprite))
    MyBase.New(service, canExecute, movedUp)
  End Sub

  Public Overrides Sub Execute(parameter As Object)
    ' Move up.
    MyBase.ExecuteMoveBase(parameter, True)
  End Sub

End Class
