Imports System.ComponentModel.DataAnnotations
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

''' <summary>
''' Contains unit tests for validation of <see cref="Models.SpriteSheetContentGeneratorOptions.Validate">
''' options</see> passed to a sprite sheet content generator.
''' </summary>
<TestClass()>
Public Class SpriteSheetContentGeneratorValidationTests

  <TestMethod()>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with ""accepted"" properties is valid.")>
  Public Sub TestNormal()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()

    ValidateOptions(options)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with a whitespace BaseClassName is invalid.")>
  Public Sub TestEmptyCssClass()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.BaseClassName = "    "

    ValidateOptions(options)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with a BaseClassName that is not a valid CSS class is invalid.")>
  Public Sub TestInvalidCssClass()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.BaseClassName = "$BAD*CSS~MOJO"

    ValidateOptions(options)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with a BaseFileName that is an invalid filename is invalid.")>
  Public Sub TestInvalidFileName()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.BaseFileName = "\#%*#(*&Y%#"

    ValidateOptions(options)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with an invalid ImageHeight is invalid.")>
  Public Sub TestInvalidHeight()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.ImageHeight = 0

    ValidateOptions(options)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with an invalid ImageWidth is invalid.")>
  Public Sub TestInvalidWidth()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.ImageWidth = 0

    ValidateOptions(options)

  End Sub

  <TestMethod()>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with an null FileExtensions collection is valid.")>
  Public Sub TestNullExtensions()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.FileExtensions = Nothing

    ValidateOptions(options)

  End Sub

#Region "FilterClassNames/HoverClassNames Validation"

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with a null FilterClassNames collection is invalid.")>
  Public Sub TestNullFilterClasses()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.FilterClassNames = Nothing

    ValidateOptions(options)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with a whitespace CSS class in its FilterClassNames collection is invalid.")>
  Public Sub TestWhitespaceFilterClasses()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.FilterClassNames = {"valid", "    "}

    ValidateOptions(options)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with an invalid CSS class in its FilterClassNames collection is invalid.")>
  Public Sub TestInvalidFilterClasses()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.FilterClassNames = {"valid", "$BAD*CSS~MOJO"}

    ValidateOptions(options)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with a null HoverClassNames collection is invalid.")>
  Public Sub TestNullHoverClasses()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.HoverClassNames = Nothing

    ValidateOptions(options)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with an invalid CSS class in its HoverClassNames collection is invalid.")>
  Public Sub TestInvalidHoverClasses()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.HoverClassNames = {"valid", "$BAD*CSS~MOJO"}

    ValidateOptions(options)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with a whitespace CSS class in its HoverClassNames collection is invalid.")>
  Public Sub TestWhitespaceHoverClasses()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.HoverClassNames = {"valid", "    "}

    ValidateOptions(options)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with an identical CSS class defined in both its FilterClassNames and HoverClassNames collections is invalid.")>
  Public Sub TestOverlappingFilterAndHoverClasses()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.HoverClassNames = {"overlap"}
    options.FilterClassNames = {"overlap"}

    ValidateOptions(options)

  End Sub

#End Region

#Region "ClassDelimiters Validation"

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with a null ClassDelimiters collection is invalid.")>
  Public Sub TestNullDelimiterClasses()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.ClassDelimiters = Nothing

    ValidateOptions(options)

  End Sub

  <TestMethod()>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with an empty ClassDelimiters collection is valid if no filter or hover classes are specified.")>
  Public Sub TestEmptyDelimiterClasses()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.ClassDelimiters = {}

    ' This is only valid if we don't have hover or filter classes
    options.FilterClassNames = {}
    options.HoverClassNames = {}

    ValidateOptions(options)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with an empty ClassDelimiters collection is invalid if filter classes are specified.")>
  Public Sub TestEmptyDelimiterClasses_RequiredByFilters()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.ClassDelimiters = {}
    options.HoverClassNames = {}

    ValidateOptions(options)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Content Generator Options Validation")>
  <Description("Tests that a set of options with an empty ClassDelimiters collection is invalid if hover classes are specified.")>
  Public Sub TestEmptyDelimiterClasses_RequiredByHovers()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.ClassDelimiters = {}
    options.FilterClassNames = {}

    ValidateOptions(options)

  End Sub

#End Region

  ''' <summary>
  ''' Validates the provided options.
  ''' </summary>
  ''' <param name="options">The options.</param>
  Private Shared Sub ValidateOptions(options As Models.SpriteSheetContentGeneratorOptions)

    Dim context As New ValidationContext(options)
    Validator.ValidateObject(options, context, True)

  End Sub

End Class