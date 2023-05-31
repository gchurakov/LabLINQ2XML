using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace LabXmlLinq
{
  internal class Program
  {
    public static void Example()
    {
      XDocument d = new XDocument(
        new XDeclaration("1.0", "windows-1251", null), new XElement("описания-книг",
          new XElement("title-info", new XElement("genre",
              new XAttribute("match", "90"),
              "sf_fantasy"),
            new XElement("author",
              new XElement("first-name", "Джон"),
              new XElement("middle-name", "Рональд Руэл"), new XElement("last-name", "Толкин")),
            new XElement("book-title", "Возвращение Короля"), new XElement("lang", "ru"),
            new XElement("sequence",
              new XAttribute("name", "Властелин Колец"),
              new XAttribute("number", "3")))));
      Console.WriteLine(d);
    }

    public static void Ex5()
    {

      //Даны имена существующего текстового файла и создаваемого XML-документа.
      //Каждая строка текстового файла содержит несколько (одно или более) слов,
      //разделен- ных ровно одним пробелом. Создать XML-документ с корне- вым элементом root,
      //элементами первого уровня line и элементами второго уровня word.
      //Элементы line соответст- вуют строкам исходного файла и не содержат дочерних тек- стовых узлов,
      //элементы word каждого элемента line содер- жат по одному слову из соответствующей строки
      //(слова рас- полагаются в порядке их следования в исходной строке).
      //Элемент line должен содержать атрибут num, равный поряд- ковому номеру строки в исходном файле,
      //элемент word дол- жен содержать атрибут num, равный порядковому номеру слова в строке.
      int i = 0;
      var text = new XDocument(new XElement("root", File
        .ReadAllText("/Users/admin/RiderProjects/LabXmlLinq/LabXmlLinq/xmllinq5.txt")
        .Split('\n')
        .Select(l => l
          .Split(' '))
        .Select(ll => new XElement("line", ll
            .Select(w => new XElement("word", w)),
          new XAttribute("num", ++i)))));
      text.Save("xmllinq5.xml");
      Console.WriteLine(text);
    }

    public static void Ex15()
    {
      //Ex15
      //Дан XML-документ, содержащий хотя бы один элемент первого уровня.
      //Для каждого элемента первого уровня найти количество его потомков,
      //имеющих не менее двух атрибутов, и вывести имя элемента первого уровня и найденное количество его потомков.
      //Элементы выводить в алфавитном порядке их имен,
      //а при совпадении имен — в порядке возрастания найденного количества потомков.

      var result = XDocument.Load("xmllinq35.xml").Root
        .Elements()
        .OrderBy(e => e.Name.ToString()) // Сортируем по имени в алфавитном порядке
        .Select(e => new
        {
          ElementName = e.Name.ToString(),
          ChildCount = e.Elements()
            .Count(c => c.Attributes().Count() >= 2) // Считаем количество потомков с двумя или более атрибутами
        })
        .OrderBy(e => e.ElementName)
        .ThenBy(e => e.ChildCount);

      foreach (var item in result)
      {
        Console.WriteLine($"Имя элемента: {item.ElementName}, Количество потомков: {item.ChildCount}");
      }

    }

    public static void Ex25()
    {
      //ex25
      //Дан XML-документ. Для всех элементов первого и второго уровня,
      //имеющих более одного атрибута, удалить все их атрибуты.

      var modifiedDoc = new XDocument(
        new XElement(XDocument.Load("xmllinq35.xml").Root.Name,
          XDocument.Load("xmllinq.xml").Root.Elements()
            .Select(element =>
            {
              if (element.Attributes().Count() > 1)
                return new XElement(element.Name);
              else
                return new XElement(element.Name,
                  element.Attributes(),
                  element.Elements());
            })
        )
      );

      modifiedDoc.Save("xmllinq25.xml");


    }

    public static void Ex35()
    {
      //Ex35. Дан XML-документ.
      //Для каждого элемента второго уровня добавить в конец списка его атрибутов атрибут
      //child-count со значением, равным количеству всех дочерних узлов этого элемента.
      //Если элемент не имеет дочерних узлов, то атрибут child-count должен иметь значение 0.

      var modifiedXml = new XDocument(
        new XElement(XDocument.Load("xmllinq5.xml").Root.Name,
          XDocument.Load("xmllinq5.xml").Root.Elements().Select(element =>
          {
            int childCount = element.Elements().Count();
            var attributesWithChildCount = element.Attributes()
              .Concat(new[] {new XAttribute("child-count", childCount)});
            return new XElement(element.Name,
              attributesWithChildCount,
              element.Nodes());
          })
        )
      );

      Console.WriteLine(modifiedXml);
      modifiedXml.Save("xmllinq35.xml");
    }

    public static void Ex45()
    {
      // Ex45. Дан XML-документ.
      //Для каждого элемента, имеющего атрибуты, добавить в начало его набора дочерних узлов
      //элемент с именем odd-attr-count и логическим значением, равным true,
      //если суммарное количество атрибутов данного элемента и всех его элементов-потомков является нечетным,
      //и false в противном случае.
      //Указание.
      //В качестве параметра конструктора XElement, определяющего значение элемента,
      //следует использовать логическое выражение; это позволит отобразить значение логической константы в соответствии со стандартом XML.

      var xmlDocument = XDocument.Load("xmllinq35.xml");

      xmlDocument
        .Descendants()
        .Where(e => e.Attributes().Any())
        .ToList()
        .ForEach(e =>
        {
          bool isOddAttrCount = (e.Attributes().Count() + e.Descendants().Elements().Count()) % 2 != 0;
          XElement oddAttrCountElement = new XElement("odd-attr-count", isOddAttrCount.ToString());
          e.AddFirst(oddAttrCountElement);
        });
      Console.WriteLine(xmlDocument);
      xmlDocument.Save("xmllinq45.xml");
    }

    public static void Ex55()
    {
      //EX 55. Дан XML-документ.
      //Преобразовать имена всех элементов второго уровня, удалив из них пространства имен
      //(для элементов других уровней пространства имен не измtнять).


      var d = new XDocument(
        new XElement(XDocument.Load("xmllinq45.xml").Root.Name,
          XDocument.Load("xmllinq45.xml").Root.Elements()
            .Select(e => new XElement(e.Name.LocalName,
              e.Elements()
                .Select(s => new XElement(s.Name.LocalName, s.Value))))));

      Console.WriteLine(d.ToString());
      d.Save("xmllinq55.xml");
    }

    public static void Ex65()
    {
      //Ex65. Дан XML-документ с информацией о клиентах фитнес-центра.
      //Образец элемента первого уровня (смысл данных тот же, что и в LinqXml61, данные сгруппированы по кодам клиентов;
      //коды клиентов, снабженные префиксом id, указываются в качестве имен элементов первого уровня):
      // <id10>
      // <info>
      // <date>2000-05-01T00:00:00</date>
      // <time>PT5H13M</time>
      // </info>
      // ...
      // </id10>

      // Преобразовать документ,
      //    сгруппировав данные по годам и изменив элементы первого уровня следующим образом:

      // <year value="2000">
      // <total-time id="10">860</total-time> ...
      // </year>

      //Значение элемента второго уровня должно быть равно общей продолжительности занятий (в минутах)
      //клиента с указанным кодом в течение указанного года.
      //Элементы первого уровня должны быть отсортированы по возрастанию номера года,
      //их дочерние элементы — по возрастанию кодов клиентов.



      var d = new XDocument(
        new XElement("root",
          XDocument.Load("xmllinq65.xml").Root
            .Elements()
            .GroupBy(e => DateTime.Parse(e.Element("info")?.Element("date")?.Value)
              .Year.ToString())
            .OrderBy(g => g.Key)
            .Select(g =>
              new XElement("year", new XAttribute("value", g.Key),
                g.Elements()
                  .OrderBy(e => e.Name.LocalName.Replace("id", ""))
                  .Select(e =>
                    new XElement("total-time", new XAttribute("id", e.Name.LocalName.Replace("id", "")),
                      (int) e.Elements("info")
                        .Sum(info => TimeSpan.Parse(info.Value).TotalMinutes))
                  )
              )
            )
        )
      );
      d.Save("xmllinq65_tr.xml");
      Console.WriteLine(d.ToString());
    }


    public static void Ex75()
    {
      //Ex75. Дан XML-документ с информацией о ценах автозаправочных станций на бензин.
      //Образец элемента первого уровня (смысл данных тот же, что и в LinqXml68, названия компании и улицы
      //, разделенные символом подчеркивания, указываются в качестве имен элементов первого уровня):
      // <Лидер_ул.Чехова> 
      //   <brand>92</brand> 
      //   <price>2200</price>
      // </Лидер_ул.Чехова>
      //Преобразовать документ, сгруппировав данные по названиям улиц и изменив элементы первого уровня следующим обра- зом:
      // <ул.Чехова>
      // <brand98 station-count="0">0</brand98> <brand95 station-count="0">0</brand95> <brand92 station-count="3">2255</brand92>
      // </ул.Чехова>
      //Имя элемента первого уровня совпадает с названием улицы,
      //имя элемента второго уровня имеет префикс brand, после которого указывается марка бензина.
      //Атрибут station-count равен количеству АЗС, расположенных на данной улице и предлагающих бензин данной марки;
      //значением элемента второго уровня является средняя цена 1 литра бензина данной марки по всем АЗС,расположенным на данной улице.
      //Средняя цена находится по следующей формуле:
      //   «суммарная цена по всем станциям»/«число станций», где операция «/» обозначает целочисленное деление.
      //   Если на данной улице отсутствуют АЗС, предлагающие бензин данной марки,
      //   то значение соответствующего элемента второго уровня и значение его атрибута station-count должны быть равны 0.
      //Элементы первого уровня должны быть отсортированы в алфавитном порядке названий улиц,
      //а их дочерние элементы — по убыванию марок бензина.

      // var result = XDocument.Load("xmllinq75.xml").Root
      //   .Elements()
      //   .GroupBy(e => e.Name.ToString().Split('_')[1]) // Группируем по названию улицы
      //   .OrderBy(g => g.Key) // Сортируем по алфавиту
      //   .Select(g =>
      //   {
      //     var streetName = g.Key;
      //     var brandElements = g.Elements()
      //       .GroupBy(e => e.Value) // Группируем по марке бензина
      //       .OrderByDescending(g => g.Key) // Сортируем по убыванию марки бензина
      //       .Select(brandGroup =>
      //       {
      //         var brandName = brandGroup.Key;
      //         var stationCount = brandGroup.Count();
      //         var totalPrice = brandGroup.Sum(e => int.Parse(e.Value));
      //         var averagePrice = totalPrice / stationCount;
      //
      //         var brandElement = new XElement($"brand{brandName}", averagePrice);
      //         brandElement.SetAttributeValue("station-count", stationCount);
      //
      //         return brandElement;
      //       });
      //
      //     var streetElement = new XElement($"ул.{streetName}", brandElements);
      //     return streetElement;
      //   });
      //
      // var newD = new XDocument(new XElement("root", result));
      // Console.WriteLine(newD.ToString());
      //transformedXml.Save("xmllinq75_tr.xml");

      var groupedData = XDocument.Load("xmllinq75.xml").Root
        .Elements()
        .GroupBy(e => GetStreetName(e.Name.ToString()))
        .OrderBy(g => g.Key)
        .Select(g => new XElement(g.Key,
          g.Elements("brand")
            .GroupBy(e => e.Value)
            .OrderByDescending(gr => gr.Key)
            .Select(gr => new XElement("brand" + gr.Key,
              new XAttribute("station-count", gr.Count()),
              gr.Count() == 0 ? 0 : gr.Sum(e => int.Parse(e.Value)) / gr.Count()))));

      

      // Создаем новый XML-документ с преобразованными данными
      XDocument resultDoc = new XDocument(new XElement("root", groupedData));

      // Выводим результат
      Console.WriteLine(resultDoc);
      resultDoc.Save("xmllinq75_tr.xml");
    }


    // Получение названия улицы из имени элемента первого уровня
    public static string GetStreetName(string elementName)
    {
      int underscoreIndex = elementName.IndexOf('_');
      return elementName.Substring(underscoreIndex + 1);
    }

    public static void Ex83()
    {
      // LinqXml83. Дан XML-документ с информацией об оценках учащихся по различным предметам. Образец элемента пер- вого уровня:
      //   <record>
      //   <class>9</class> <name>Степанова Г.Б.</name> <subject>Уизика</subject> <mark>4</mark>
      //   </record>
      //   Здесь class — номер класса (целое число от 7 до 11), name — фамилия и инициалы учащегося (инициалы не содержат пробелов и отделяются от фамилии одним пробелом), subject — название предмета, не содержащее пробелов, mark — оценка (целое число в диапазоне от 2 до 5). Полных однофамильцев (с совпадающей фамилией и инициалами) среди учащихся нет. Преобразовать документ, изменив эле- менты первого уровня следующим образом:
      //   <mark subject="Уизика">
      //   <name class="9">Степанова Г.Б.</name> <value>4</value>
      //   </mark>
      //   Порядок следования элементов первого уровня не изменять.
      
      XDocument document = XDocument.Load("mxllinq83.xml");

      // Создаем новый XML-документ с преобразованными элементами
      XDocument tdoc = new XDocument(
        new XElement("root",
          document.Root.Elements()
            .Select(record => new XElement("mark",
              new XAttribute("subject", record.Element("subject").Value),
              new XElement("name",
                new XAttribute("class", record.Element("class").Value),
                record.Element("name").Value),
              new XElement("value", record.Element("mark").Value))
            )
        )
      );
      Console.WriteLine(tdoc.ToString());
      tdoc.Save("mxllinq83_tr.xml");
    }

    public static void Ex84()
    {
      // Дан XML-документ с информацией об оценках учащихся по различным предметам. Образец элемента пер- вого уровня
      // (смысл данных тот же, что и в LinqXml83):
      
      //   <pupil class="9" name="Степанова Г.Б."> <subject>Уизика</subject> <mark>4</mark>
      //   </pupil>
      
      //   Преобразовать документ, изменив элементы первого уровня следующим образом:
      
      //   <class9 name="Степанова Г.Б." subject="Уизика">4</class9>
      
      //   Имя элемента должно иметь префикс class, после которого указывается номер класса.
      // Элементы должны быть отсорти- рованы по возрастанию номеров классов, для одинаковых номеров классов —
      // в алфавитном порядке фамилий и ини- циалов учащихся, для каждого учащегося — в алфавитном порядке названий предметов,
      // а для одинаковых предметов — по возрастанию оценок.
      
      XDocument doc = XDocument.Load("xmllinq84.xml");

      var tdoc = doc.Root.Elements("pupil")
            .Select(pupil => new XElement(
              "class" + pupil.Attribute("class").Value,
              new XAttribute("name", pupil.Attribute("name").Value),
              new XAttribute("subject", pupil.Element("subject").Value),
              pupil.Element("mark")))
            .OrderBy(element => int.Parse(element.Name.LocalName.Substring(5))) // Сортировка по номеру класса
            .ThenBy(element => element.Attribute("name").Value) // Сортировка по имени учащегося
            .ThenBy(element => element.Attribute("subject").Value) // Сортировка по названию предмета
            .ThenBy(element => int.Parse(element.Value)); // Сортировка по оценке

      var res = new XDocument(
        new XElement("root", tdoc));

      Console.WriteLine(res.ToString());
      res.Save("xmllinq84.xml");
    }
    public static void Ex85()
    {
      //Ex85. Дан XML-документ с информацией об оценках учащихся по различным предметам.
      //Образец элемента пер- вого уровня (смысл данных тот же, что и в LinqXml83):
      //   <info class="9" name="Степанова Г.Б." subject="Уизика" mark="4" />
      //Преобразовать документ,
      //  выполнив группировку данных по номеру класса, в пределах каждого класса — по учащимся,
      //  а для каждого учащегося — по предметам.
      //Изменить элементы первого уровня следующим образом:
      
      // <root>
      //   <class number="9">
      //   <pupil name="Степанова Г.Б.">
      //   <subject name="Физика">
      //   <mark>4</mark>
      //   </subject>
      //   <subject name="Математика">
      //   <mark>5</mark>
      //   </subject>
      //   </pupil>
      //   <pupil name="Иванов А.С.">
      //   <subject name="Физика">
      //   <mark>3</mark>
      //   </subject>
      //   <subject name="Математика">
      //   <mark>4</mark>
      //   </subject>
      //   </pupil>
      //   </class>
      // </root>

      // Элементы первого уровня должны быть отсортированы по возрастанию номеров классов,
      // а их дочерние элементы — в алфавитном порядке фамилий и инициалов учащихся.
      // Элементы третьего уровня, имеющие общего родителя,
      // должны быть отсортированы в алфавитном порядке названий пред- метов,
      // а элементы четвертого уровня, имеющие общего ро- дителя, должны быть отсортированы по убыванию оценок.


      XDocument doc = XDocument.Load("xmllinq85.xml");

      // Выполнение группировки данных по классам, учащимся и предметам
      var groupedData = doc.Root
        .Elements("info")
        .OrderBy(e => (int) e.Attribute("class"))
        .ThenBy(e => (string) e.Attribute("name"))
        .GroupBy(
          e => new {Class = (int) e.Attribute("class"), Pupil = (string) e.Attribute("name")},
          e => new {Subject = (string) e.Attribute("subject"), Mark = (int) e.Attribute("mark")}
        );
      Console.WriteLine("g:", groupedData.ToString());

      // Создание нового XML-документа с измененной структурой
      XDocument newDoc = new XDocument(
        new XElement("root",
          groupedData.Select(g =>
            new XElement("class",
              new XAttribute("number", g.Key.Class),
              new XElement("pupil",
                new XAttribute("name", g.Key.Pupil),
                g.Select(s =>
                  new XElement("subject",
                    new XAttribute("name", s.Subject),
                    new XElement("mark", s.Mark)
                  )
                ).OrderByDescending(s => s.Element("mark").Value)
              )
            )
          ).OrderBy(e => (int) e.Attribute("number"))
        )
      );

      // Сохранение нового XML-документа в файл или другое место назначения
      newDoc.Save("xmllinq85_tr.xml");
      Console.WriteLine(newDoc.ToString());
    }

    public static void Ex86()
    {
      // LinqXml86. Дан XML-документ с информацией об оценках учащихся по различным предметам.
      // Образец элемента пер- вого уровня (смысл данных тот же, что и в LinqXml83):
      
      //   <pupil name="Степанова Г.Б." class="9"> <info mark="4" subject="Уизика" />
      //   </pupil>
      
      //   Преобразовать документ, выполнив группировку данных по учащимся и изменив элементы первого уровня следующим образом:
      
      //   <Степанова_Г.Б. class="9">
      //      <mark4 subject="Уизика" /> ...
      //   </Степанова_Г.Б.>
      
      // Имя элемента первого уровня совпадает с фамилией и ини- циалами учащегося
      // (пробел между фамилией и инициалами заменяется символом подчеркивания),
      // имя элемента второго уровня должно иметь префикс mark, после которого указыва- ется оценка.
      // Элементы первого уровня должны быть отсор- тированы в алфавитном порядке фамилий и инициалов учащихся,
      // их дочерние элементы — по убыванию оценок, а для одинаковых оценок — в алфавитном порядке названий предметов.

      
      var doc = XDocument.Load("xmllinq86.xml").Root.Elements()
        .OrderBy(p => p.Attribute("name").Value)
        .Select(p =>
          new XElement(String.Join("_", p.Attribute("name").Value.ToString().Split(' ')),
            new XAttribute("class", p.Attribute("class").Value),
            p.Elements("info")
              .OrderByDescending(info => (int)info.Attribute("mark"))
              .ThenBy(info => info.Attribute("subject").Value)
              .Select(info =>
                new XElement("mark" + info.Attribute("mark").Value.ToString(),
                  new XAttribute("subject", info.Attribute("subject").Value.ToString())
                )
              )
          )
        );
      var doct = new XDocument(
        new XElement("root", doc));

      Console.WriteLine(doc.ToString());
    }

    public static void Main(string[] args)
    {

    }

  
  }
}