# Desafio ICI - Samuel

Projeto Realizado para o Desafio para o Instituto Cidades Inteligentes seguindo os requerimentos passados.

Espero que gostem e possamos ter uma conversa para nos conhecermos :)

---

### Notas sobre as decisões tomadas para o projeto, onde o objetivo foi manter a simplicidade de forma organizada, entendível e clara.

A estrutura do projeto foi pensada para ser escalável, organizada e de fácil manutenção, utilizando os seguintes conceitos:

*   **Arquitetura do Projeto segue o Padrão MVC**: Como pedido no desafio, a arquitetura é MVC.

*   **Organização do projeto Orientada a Features (Feature-Sliced Architecture)**: Optei pela ideia da Vertical Slice Architecture para organização das pastas, por gosto pessoal e também por achar bem eficiente organizar o código mais baseado nas features do que, por exemplo, numa N-TIER onde a separação é por módulos "técnicos".

*   **Padrão CQRS (Command Query Responsibility Segregation)**: A lógica de negócio é separada em dois tipos de operações:
    *   **Commands**: Ações que alteram o estado dos dados (criar, editar, deletar). Cada comando tem seu próprio `Handler` (ex: `CreateNewsHandler`).
    *   **Queries**: Ações que apenas leem os dados (listar, buscar por ID). Cada consulta também tem seu `Handler` (ex: `ListNewsHandler`).
    Essa separação simplifica as responsabilidades, otimiza as operações de leitura e escrita e torna o código mais limpo.

*   **Injeção de Dependência (Dependency Injection - DI)**: O projeto utiliza o contêiner de DI nativo do ASP.NET Core. As dependências são definidas através de interfaces (ex: `ICreateNewsHandler`) e injetadas nos construtores (como nos `Controllers`), o que promove baixo acoplamento, facilita a substituição de implementações e torna o código mais testável.

*   **Entity Framework Core (Code-First)**: A persistência de dados é gerenciada com o EF Core, utilizando a abordagem "Code-First". As entidades de domínio (na pasta `Domain`) são a fonte da verdade, e as `Migrations` geram e atualizam o esquema do banco de dados a partir delas. Para simplificar a configuração, o projeto utiliza **SQLite** como banco de dados, optei por ele pela simplicidade, por ser um projeto demo.

*   **Testes Unitários dos Handlers**: Testes unitários para todos os handlers, garantindo que funcionem como esperado.

*   **Boas práticas/padrões que gosto que foram implementadas**:
 - Utilização de código assíncrono para não bloquear thread principal(I/O), favorecendo a escalabilidade;
 - Uso e abuso do early return pattern para evitar multiplas identações do código;
 - Utilização das features do MVC para passar valores de mensagem de sucesso/erro e serem renderizadas ao retornar a view;
 - Utilização de interfaces para manter desacoplamento de classes, e assim as classes não dependerem de classes concretas mas sim de interfaces, isso facilitando, por exemplo, testabilidade, onde ao testar a classe que possui a dependencia, conseguiríamos criar ela e mockar o objeto da sua devida interface para simular um determinado comportamento.
 - Paginação server side para melhor performance;
 - Uso de asNoTracking em queries(do entityFramework) e instrução clara de quais colunas retornarem para melhor performance;
 - Isolamento de adição de serviços(DI e App Context) para deixar o Program.cs limpo e isolar as responsabilidades;
 - Utilização do AppSettings para determinar caminho utilizado pelo SqLite;

*   **Boas práticas/padrões  que acho interessante e que foram não implementadas(conscientemente)**:
- Optei por não utilizar Repository Pattern, uma vez que o entity já nos fornece o padrão Unit Of Work e acreditei que ia ser uma camada apenas para abstrair uma complexidade desnecessária. Utilizando a interface do AppDbContext permite mockar comportamentos para testar os devidos handlers.
 - Poderia seguir alguns pressupostos da Clean Arch para separar as pastas em Projetos, como por exemplo: Application, Infra/DataAccess, CrossCuting, e por aí vai. Não optei por essa abordagem por achar "over" para um projeto demo, e também acreditar que a estrutura implementada segue um bom padrão, deixando o código fácil de evoluir, entendível e organizado.
 - Também Poderia modelar um domínio mais rico, seguindo os conceitos de DDD, mas também achei desnecessário tendo em vista que a estrutura atual é simples e elegante o suficiente. Se eu fosse seguir esse caminho, iria adicionar um projeto Domain com os domínios/entidades e talvez usar NotificationPattern para validar as entidades(gosto da Flunt, que inclusive o criador é o Prof Andre Baltieri, com quem aprendi muito quando estava começando).
 - Gosto bastante de utilizar a FluentValidation para validações de classes, mas utilizando MVC e com um projeto "simples" optei por DataAnnotations Front + ModelState(Back) pela simplicidade e efetividade.

---

### Como Executar a Aplicação

Siga os passos abaixo para configurar e executar o projeto localmente.

**Pré-requisitos:**
*   [.NET 8 SDK]
*   (Opcional) Uma IDE como Visual Studio 2022 ou VS Code.

**Passos:**

1.  **Instale as ferramentas do Entity Framework Core (se ainda não tiver):**
    ```sh
    dotnet tool install --global dotnet-ef
    ```

3.  **Restaure as dependências do projeto:**
    Navegue até a pasta raiz do projeto e execute o comando:
    ```sh
    dotnet restore
    ```

4.  **Aplique as Migrations para criar o banco de dados:**
    Este comando irá criar o arquivo de banco de dados `database.db` na pasta `Data/DB/` do projeto.
    ```sh
    dotnet ef database update
    ```

5.  **Execute a aplicação:**
    ```sh
    dotnet run
    ```
5.  **Rodando os tests:**
    ```sh
    dotnet test
    ```
