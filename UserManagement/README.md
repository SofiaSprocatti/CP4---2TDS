# Vehicle Rental Management System

Sistema completo de gerenciamento de locação de veículos desenvolvido com .NET 8, seguindo os princípios de Clean Architecture, Domain-Driven Design (DDD) e Clean Code.

## Integrantes do Grupo

- Sofia Sprocatti -  RM99208

## Descrição do Domínio

O sistema gerencia o ciclo completo de locação de veículos, incluindo:

- **Cadastro de usuários** com documentos (CPF/CNPJ) e CNH
- **Catálogo de modelos de veículos** com especificações e preços
- **Frota de veículos** com controle de status e manutenção
- **Processo de locação** desde a reserva até a devolução
- **Documentos** associados a usuários, veículos e locações
- **Regras de negócio** para elegibilidade e restrições

## Arquitetura

O projeto segue os princípios da Clean Architecture, organizando o código em camadas bem definidas:

```
📦 src
 ┣ 📂 Api             -> Controllers, configuração da API, middleware
 ┣ 📂 Application     -> Casos de uso, DTOs, serviços de aplicação
 ┣ 📂 Domain          -> Entidades, Value Objects, agregados, interfaces
 ┗ 📂 Infrastructure  -> Repositórios, EF Core, persistência
```

### Responsabilidades das Camadas

- **Domain**: Contém as regras de negócio, entidades ricas, value objects e agregados
- **Application**: Implementa os casos de uso e orchestração de serviços
- **Infrastructure**: Responsável pela persistência, acesso a dados e serviços externos
- **Api**: Controllers, middleware, configuração da Web API e logging

## Domain-Driven Design (DDD)

### Entidades Ricas

- **User**: Gerencia dados pessoais, documentos, CNH e elegibilidade para locação
- **Vehicle**: Controla status, manutenção, restrições e disponibilidade
- **VehicleModel**: Define modelos com especificações e preços
- **Rental**: Agregado raiz que gerencia o ciclo completo de locação
- **Document**: Representa documentos associados ao sistema

### Value Objects

- **Email**: Validação e formatação de emails
- **Name**: Composição de nomes com validações
- **LicensePlate**: Validação de placas brasileiras (antiga e Mercosul)
- **Chassis**: Validação de chassi/VIN
- **CNH**: Carteira de habilitação com categoria e validade
- **DocumentNumber**: CPF e CNPJ com validações

### Agregados e Regras de Negócio

- **User**: Elegibilidade baseada em documentos válidos e CNH ativa
- **Vehicle**: Estados bem definidos (disponível, alugado, manutenção, restrito)
- **Rental**: Controle de ciclo de vida com cálculos automáticos de valores
- **Validações**: CNH adequada para tipo de veículo, disponibilidade, etc.

## Tecnologias Utilizadas

- **.NET 9** - Framework principal
- **Entity Framework Core** - ORM e acesso a dados
- **SQL Server** - Banco de dados
- **Swagger/OpenAPI** - Documentação da API
- **Serilog** - Logging estruturado
- **C# 12** - Linguagem com features modernas

## Funcionalidades Implementadas

### 🚗 Gestão de Veículos

- **Modelos de Veículos**: CRUD completo com marca, modelo, ano, motor, combustível
- **Veículos**: Cadastro com placa, chassi, quilometragem e status
- **Operações**: Disponibilizar, enviar para manutenção, restringir, desativar

### 👥 Gestão de Usuários

- **Cadastro**: Nome, email, documentos (CPF/CNPJ), CNH
- **Validações**: Email único, CNH válida, elegibilidade para locação
- **Operações**: Ativar/desativar, adicionar documentos

### 📋 Gestão de Locações

- **Ciclo Completo**: Criação → Confirmação → Ativo → Finalização
- **Cálculos**: Valores automáticos, multas por atraso
- **Operações**: Estender prazo, devolver, cancelar, marcar como atrasado

### 📄 Gestão de Documentos

- **Tipos**: CNH, contratos, documentos de veículos, seguros
- **Associações**: Usuários, veículos, locações
- **Controle**: Validade, expiração, ativação/desativação

### 🧪 Sistema de Testes

- **Controller de Testes**: Endpoint único para testar todas as funcionalidades
- **Dados Randomizados**: Geração automática de dados de teste
- **Status do Sistema**: Relatório completo do estado atual

## Endpoints da API

### Usuários (`/api/users`)

- `GET /api/users` - Lista usuários (com filtro de ativos)
- `GET /api/users/{id}` - Busca usuário por ID
- `POST /api/users` - Cria novo usuário
- `PUT /api/users/{id}` - Atualiza usuário
- `DELETE /api/users/{id}` - Remove usuário
- `PATCH /api/users/{id}/activate` - Ativa usuário
- `PATCH /api/users/{id}/deactivate` - Desativa usuário
- `POST /api/users/{id}/document` - Adiciona documento ao usuário
- `POST /api/users/{id}/driver-license` - Adiciona CNH ao usuário
- `GET /api/users/{id}/can-rent/{vehicleType}` - Verifica elegibilidade

### Modelos de Veículos (`/api/vehiclemodels`)

- `GET /api/vehiclemodels` - Lista modelos (com filtro de ativos)
- `GET /api/vehiclemodels/{id}` - Busca modelo por ID
- `POST /api/vehiclemodels` - Cria novo modelo
- `PUT /api/vehiclemodels/{id}` - Atualiza modelo
- `DELETE /api/vehiclemodels/{id}` - Remove modelo
- `PATCH /api/vehiclemodels/{id}/activate` - Ativa modelo
- `PATCH /api/vehiclemodels/{id}/deactivate` - Desativa modelo

### Veículos (`/api/vehicles`)

- `GET /api/vehicles` - Lista veículos (com filtros de status, tipo, disponíveis)
- `GET /api/vehicles/{id}` - Busca veículo por ID
- `POST /api/vehicles` - Cadastra novo veículo
- `PUT /api/vehicles/{id}/mileage` - Atualiza quilometragem
- `PATCH /api/vehicles/{id}/make-available` - Disponibiliza veículo
- `PATCH /api/vehicles/{id}/maintenance` - Envia para manutenção
- `PATCH /api/vehicles/{id}/restrict` - Restringe veículo
- `PATCH /api/vehicles/{id}/deactivate` - Desativa veículo
- `DELETE /api/vehicles/{id}` - Remove veículo

### Locações (`/api/rentals`)

- `GET /api/rentals` - Lista locações (com filtro de status)
- `GET /api/rentals/{id}` - Busca locação por ID
- `GET /api/rentals/user/{userId}` - Locações de um usuário
- `GET /api/rentals/vehicle/{vehicleId}` - Locações de um veículo
- `POST /api/rentals` - Cria nova locação
- `PATCH /api/rentals/{id}/confirm` - Confirma locação (entrega)
- `PATCH /api/rentals/{id}/complete` - Finaliza locação (devolução)
- `PATCH /api/rentals/{id}/cancel` - Cancela locação
- `PATCH /api/rentals/{id}/extend` - Estende prazo
- `PATCH /api/rentals/{id}/mark-overdue` - Marca como atrasado

### Testes (`/api/test`)

- `POST /api/test/run-all-tests` - Executa todos os testes
- `POST /api/test/test-users` - Testa fluxo de usuários
- `POST /api/test/test-vehicle-models` - Testa modelos de veículos
- `POST /api/test/test-vehicles` - Testa veículos
- `POST /api/test/test-rentals` - Testa locações
- `GET /api/test/system-status` - Status completo do sistema

## Como Executar

### Pré-requisitos

- .NET 9 SDK
- SQL Server ou SQL Server LocalDB
- Visual Studio 2022 ou VS Code

### Passos

1. **Clone o repositório:**

```bash
git clone [URL_DO_REPOSITORIO]
cd UserManagement
```

2. **Configure a string de conexão** no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=VehicleRentalDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

3. **Navegue até o projeto da API:**

```bash
cd src/Api
```

4. **Restaure as dependências:**

```bash
dotnet restore
```

5. **Execute a aplicação:**

```bash
dotnet run
```

6. **Acesse o Swagger UI:**

```
https://localhost:5xxx/swagger
```

7. **Execute os testes do sistema:**

- Acesse `POST /api/test/run-all-tests` no Swagger
- Ou teste individualmente cada módulo

## Estrutura do Banco de Dados

### Tabelas Principais

- **Users**: Usuários com nome, email, documento, CNH
- **VehicleModels**: Modelos com marca, modelo, ano, motor, preço
- **Vehicles**: Veículos com placa, chassi, status, quilometragem
- **Rentals**: Locações com período, valores, status
- **Documents**: Documentos associados ao sistema

### Relacionamentos

- User → Rentals (1:N)
- Vehicle → Rentals (1:N)
- VehicleModel → Vehicles (1:N)
- Documents → User/Vehicle/Rental (N:1)

## Logging e Monitoramento

### Sistema de Logs Implementado

- **Request/Response Logging**: Todas as requisições são logadas
- **Business Logic Logging**: Operações de negócio com contexto
- **Error Logging**: Tratamento estruturado de erros
- **Performance Logging**: Duração de operações críticas

### Níveis de Log

- **Information**: Operações normais e fluxo de dados
- **Warning**: Situações que merecem atenção
- **Error**: Erros que precisam de intervenção
- **Debug**: Informações detalhadas para desenvolvimento

## Principios Aplicados

### Clean Code

- **SRP**: Cada classe tem uma única responsabilidade
- **DRY**: Código reutilizável sem duplicação
- **KISS**: Soluções simples e diretas
- **YAGNI**: Implementação apenas do necessário
- **Meaningful Names**: Nomes claros e intencionais
- **Small Functions**: Métodos pequenos e focados

### SOLID

- **S**: Single Responsibility Principle
- **O**: Open/Closed Principle
- **L**: Liskov Substitution Principle
- **I**: Interface Segregation Principle
- **D**: Dependency Inversion Principle

### Padrões Utilizados

- **Repository Pattern**: Abstração do acesso a dados
- **Dependency Injection**: Inversão de controle
- **DTO Pattern**: Transferência de dados entre camadas
- **Value Object Pattern**: Objetos imutáveis com validações
- **Rich Domain Model**: Entidades com comportamento
- **Aggregate Pattern**: Consistência transacional

## Exemplos de Uso

### 1. Criar e Configurar Usuário

```http
POST /api/users
{
  "firstName": "João",
  "lastName": "Silva",
  "email": "joao.silva@email.com"
}

POST /api/users/{userId}/document
{
  "documentValue": "123.456.789-00",
  "documentType": "CPF"
}

POST /api/users/{userId}/driver-license
{
  "cnhNumber": "12345678901",
  "expiryDate": "2028-12-31",
  "category": "B"
}
```

### 2. Cadastrar Modelo e Veículo

```http
POST /api/vehiclemodels
{
  "brand": "Toyota",
  "model": "Corolla",
  "year": 2023,
  "fuelType": "Flex",
  "engine": "1.8",
  "dailyRate": 150.00
}

POST /api/vehicles
{
  "licensePlate": "ABC-1234",
  "chassis": "1HGBH41JXMN109186",
  "vehicleModelId": "{modelId}",
  "type": 1
}
```

### 3. Realizar Locação Completa

```http
POST /api/rentals
{
  "userId": "{userId}",
  "vehicleId": "{vehicleId}",
  "startDate": "2024-01-15",
  "endDate": "2024-01-20"
}

PATCH /api/rentals/{rentalId}/confirm

PATCH /api/rentals/{rentalId}/complete
{
  "finalMileage": 25000,
  "notes": "Veículo devolvido em perfeitas condições"
}
```

### 4. Executar Testes Automáticos

```http
POST /api/test/run-all-tests
```

## Integração com MongoDB

Para usar MongoDB em vez de SQL Server:

1. **Substitua o provider** no `Program.cs`:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMongoDB(connectionString, databaseName));
```

2. **Ajuste as configurações** de entidade para MongoDB
3. **Atualize a string de conexão** para MongoDB

## Monitoramento e Observabilidade

- **Health Checks**: Endpoint `/api/test/system-status`
- **Structured Logging**: Logs estruturados com contexto
- **Request Tracing**: Rastreamento de requisições
- **Error Handling**: Tratamento padronizado de erros

## Contribuição

1. Siga os princípios de Clean Code e SOLID
2. Mantenha a cobertura de testes
3. Use commits semânticos (conventional commits)
4. Documente mudanças significativas

---

**Sistema desenvolvido para o CP4 - Clean Code, DDD e Clean Architecture**
**Disciplina**: Desenvolvimento de Software - 2TDS
**Ano**: 2025
