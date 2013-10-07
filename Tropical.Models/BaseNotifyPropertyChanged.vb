Imports System.ComponentModel
Imports System.Linq.Expressions

''' <summary>
''' A base class for simplified implementation of
''' <see cref="INotifyPropertyChanged" /> and <see cref="INotifyPropertyChanging" />.
''' </summary>
Public MustInherit Class BaseNotifyPropertyChanged
  Implements INotifyPropertyChanged
  Implements INotifyPropertyChanging

  ''' <summary>
  ''' Occurs when a property value changes.
  ''' </summary>
  Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

  ''' <summary>
  ''' Occurs when a property value is changing.
  ''' </summary>
  Public Event PropertyChanging As PropertyChangingEventHandler Implements INotifyPropertyChanging.PropertyChanging

  ''' <summary>
  ''' Notifies event listeners that a property has changed.
  ''' </summary>
  ''' <param name="propertyName">The name of the property that changed.</param>
  Protected Overridable Sub NotifyPropertyChanged(propertyName As [String])
    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
  End Sub

  ''' <summary>
  ''' Notifies event listeners that a property is changing.
  ''' </summary>
  ''' <param name="propertyName">The name of the property that is changing.</param>
  Protected Overridable Sub NotifyPropertyChanging(propertyName As [String])
    RaiseEvent PropertyChanging(Me, New PropertyChangingEventArgs(propertyName))
  End Sub

  ''' <summary>
  ''' Raises the <see cref="PropertyChanged" /> event for the property.
  ''' </summary>
  ''' <typeparam name="TProperty">The type of the property.</typeparam>
  ''' <param name="property">The property.</param>
  Protected Sub RaisePropertyChanged(Of TProperty)([property] As Expression(Of Func(Of TProperty)))
    NotifyPropertyChanged(GetPropertyName([property]))
  End Sub

  ''' <summary>
  ''' Raises the <see cref="PropertyChanging" /> event for the property.
  ''' </summary>
  ''' <typeparam name="TProperty">The type of the property.</typeparam>
  ''' <param name="property">The property.</param>
  Protected Sub RaisePropertyChanging(Of TProperty)([property] As Expression(Of Func(Of TProperty)))
    NotifyPropertyChanging(GetPropertyName([property]))
  End Sub

  ''' <summary>
  ''' Gets the name of the property.
  ''' </summary>
  ''' <typeparam name="TProperty">The type of the property.</typeparam>
  ''' <param name="property">The property.</param>
  ''' <returns>The name of the property.</returns>
  Private Function GetPropertyName(Of TProperty)([property] As Expression(Of Func(Of TProperty))) As String
    Dim lambda = DirectCast([property], LambdaExpression)

    Dim memberExpression As MemberExpression = If(TypeOf lambda.Body Is UnaryExpression, DirectCast(DirectCast(lambda.Body, UnaryExpression).Operand, MemberExpression), DirectCast(lambda.Body, MemberExpression))

    Return memberExpression.Member.Name
  End Function

End Class