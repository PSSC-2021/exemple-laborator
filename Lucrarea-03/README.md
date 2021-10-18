# Lucrarea 3: Implementarea unui „workflow” DDD 

**Context**: căruciorul de cumpărături pentru un magazin virtual. 

**Obiective**: reprezentarea valorilor optionale, tratarea erorilor

**Sarcina 1**

Analizați și rulați soluția din directorul exemple. Identificați elementele noi vis-a-vis de modul în care este scris și organizat codul sursă.

**Sarcina 2**

în contextul workflow-ului pentru plasarea unei comenzi folositi:
* Option < T > [4] pentru a reprezenta rezoltatul conversiei de la string la codul produsului, respectiv cantitatea
* Try < T >[5] sau TryAsync < T >[6] pentru a reprezenta rezultatele următoarelor funcții
    * verificarea existenței produsului pe baza codului de produs
    * verificarea stocului
    * verificarea adresei de livrare
* folositi expresii LINQ pentru a combina mai multe rezultate de tipul Option < T >, respectiv mai multe rezultate de tipul Try < T >/TryAsync < T >

**Referințe**

[1] https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/discriminated-unions 

[2] https://www.nuget.org/packages/CSharp.Choices/

[3] Scott Wlaschin, [Domain Modeling Made Functional](https://www.amazon.com/Domain-Modeling-Made-Functional-Domain-Driven-ebook/dp/B07B44BPFB/ref=sr_1_1?dchild=1&keywords=Domain+Modeling+Made+Functional&qid=1632338254&sr=8-1), Pragmatic Bookshelf, 2018  
[4] https://github.com/louthy/language-ext#option
[5] https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Try_A.htm
[6] https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/TryAsync_A.htm
