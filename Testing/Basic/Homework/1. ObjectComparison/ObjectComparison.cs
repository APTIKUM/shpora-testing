using NUnit.Framework;
using NUnit.Framework.Legacy;
using FluentAssertions;

namespace HomeExercise.Tasks.ObjectComparison;
public class ObjectComparison
{
    [Test]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void CheckCurrentTsar()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // 1. BeEquivalentTo сообщает путь к отличающемуся полю (например, Parent.Age),
        // ожидаемое и фактическое значения, что упрощает поиск причины падения теста.
        // 2. Поля и вложенные объекты сравниваются автоматически: при добавлении поля
        // в Person не нужно дописывать AreEqual.
        //
        // Можно было написать Excluding(person => person.ID) и Excluding(person => person.Parent.ID) - тест бы прошел
        // Но если мы добавим других царей в род - то тесты начнут падать
        // поэтому я написал Excluding через IMemberInfo, чтобы игнорировать Id на любом уровне рекурсии
        actualTsar.Should().BeEquivalentTo(expectedTsar, option => option
            .Excluding(member => member.DeclaringType == typeof(Person)
                                 && member.Name == nameof(Person.Id)));
    }

    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void CheckCurrentTsar_WithCustomEquality()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // AreEqual возвращает только bool, поэтому при несовпадении эта проверка
        // сообщает лишь, что ожидалось true, а получено false, без указания поля и значений.
        //
        // Список полей и рекурсию приходится поддерживать вручную: новое поле Person
        // легко забыть добавить в AreEqual, и тест не заметит различий в нём.
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
    }

    private bool AreEqual(Person? actual, Person? expected)
    {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }
}
