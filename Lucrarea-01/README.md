# Lucrarea 1: Crearea unui sistem de tipuri pentru un model Domain Driven Design (DDD) 

**Context**: căruciorul de cumpărături pentru un magazin virtual. 

**Obiective**: înțelegerea tipului de date choice/discriminated union [1,2], înțelegerea conceptelor de value type și entity type, construirea unui sistem de tipuri [3] specific pentru un anumit domeniu 

**Sarcina 1**

Analizați și rulați soluția din directorul exemple. Identificați elementele noi vis-a-vis de modul în care este scris și organizat codul sursă.

**Sarcina 2**

Implementarea unui sistem de tipuri pentru a reprezenta un cărucior de cumpărături și realizarea unei aplicații consolă care să folosească acele tipuri. 
Sistemul de tipuri trebuie să folosească: 
* un choice type pentru a reprezenta un cărucior în următoarele stările: gol, nevalidat, validate, plătit.  
* value type pentru a reprezenta cantitatea produselor comandate, codul produsului, adresa 
* entity type pentru a reprezenta căruciorul de cumpărături, clientul 

Aplicația consolă trebuie să permită crearea unui cărucior gol, adăugarea de produse în cărucior, trecerea unui cărucior dintr-o stare în altă fără a aplica validări. 

**Referințe**

[1] https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/discriminated-unions 

[2] https://www.nuget.org/packages/CSharp.Choices/

[3] Scott Wlaschin, [Domain Modeling Made Functional](https://www.amazon.com/Domain-Modeling-Made-Functional-Domain-Driven-ebook/dp/B07B44BPFB/ref=sr_1_1?dchild=1&keywords=Domain+Modeling+Made+Functional&qid=1632338254&sr=8-1), Pragmatic Bookshelf, 2018  
