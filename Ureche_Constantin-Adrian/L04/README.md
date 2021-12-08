# Lucrarea 4: Interacțiunea cu baza de date

**Context**: căruciorul de cumpărături pentru un magazin virtual. 

**Obiective**: citirea/scrierea din/în baza de date

**Sarcina 1**

Analizați și rulați soluția din directorul exemple. Identificați elementele noi vis-a-vis de modul în care este scris și organizat codul sursă.

**Sarcina 2**

În contextul workflow-ului pentru plasarea unei comenzi realizați următoarele:
* creați o baza de date cu următoarele tabele: Product(ProductId, Code, Stoc), OrderHeader(OrderId, Address, Total), OrderLine(OrderLineId, ProductId, Quantity, Price)
* înainte de a executa workflow-ul încărcați starea din baza de date
* implementați funcțiile de verificare a existenței produsului și stocului astfel încât să folosească informații din baza de date
* după executare workflow-ului salvați rezultatul în baza de date

**Referințe**

[1] https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/discriminated-unions 

[2] https://www.nuget.org/packages/CSharp.Choices/

[3] Scott Wlaschin, [Domain Modeling Made Functional](https://www.amazon.com/Domain-Modeling-Made-Functional-Domain-Driven-ebook/dp/B07B44BPFB/ref=sr_1_1?dchild=1&keywords=Domain+Modeling+Made+Functional&qid=1632338254&sr=8-1), Pragmatic Bookshelf, 2018  
