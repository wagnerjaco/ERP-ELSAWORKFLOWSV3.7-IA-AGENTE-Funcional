# Projeto de estudo aplicando soluções reais a um cenario empresario, com BPMN, orquestração e IA integrada Local e externa

# ERP Elsa Workflows
Sistema ERP completo desenvolvido em **.NET 9** com **Blazor WebAssembly**, integrado ao **Elsa Workflows 3.6.1** para automação de processos de negócio e um **assistente de IA** ("ConsultorERP") com chamada de funções.
## Funcionalidades
- **Gestão de Produtos**: Cadastro completo com propriedades fiscais (NCM, CEST, CFOP, ICMS, PIS, COFINS), controle de estoque mínimo/máximo e ponto de ressuprimento
- **Gestão de Fornecedores**: Cadastro com CPF/CNPJ, endereço e status (Ativo/Inativo/Bloqueado)
- **Marcas e Categorias**: Organização de produtos por tipo (Produto/Serviço/Matéria-Prima)
- **Pedidos de Compra**: Criação, gerenciamento e aprovação com fluxo de email automatizado via Elsa Workflows
- **Automação de Workflows**: Motor Elsa Workflows com atividades personalizadas (VerificaEstoque, AuthorizeFlow), multi-tenancy e suporte a scripts C#, JS, Python e Liquid
- **Chatbot de IA**: Assistente "ConsultorERP" com suporte a múltiplos provedores LLM (OpenRouter, Ollama) e skills para consultar estoque, pedidos e fornecedores via tool calling
- **Dashboard Analítico**: Gráficos de donut, barras, resumo financeiro, indicadores e atalhos rápidos
- **Interface Desktop-like**: Sistema de janelas (WinBox) com múltiplas páginas simultâneas
- **Autenticação JWT**: Roles de ADMIN, USUARIO e LEITURA
- **PWA**: Suporte a Progressive Web App
## Stack Tecnológica
| Camada | Tecnologia |
|--------|-----------|
| Backend | C# .NET 9, ASP.NET Core Web API |
| Frontend | Blazor WebAssembly .NET 9 |
| Workflow | Elsa Workflows 3.6.1 |
| ORM | Entity Framework Core 9 |
| Banco | SQL Server |
| IA | Semantic Kernel, OpenAI-compatible API |
| Auth | JWT Bearer + BCrypt |
| Email | MailKit |
## Arquitetura
- **Clean Architecture**: separação em camadas Domain, Application, Infrastructure, API e Frontend
- **Servidor de Workflows**: instância separada do Elsa com atividades customizadas, multi-tenancy e agentes de IA
- **Orquestrador de Chat**: sistema multi-provedor com fallback automático entre LLMs
Topics (tags):
erp, elsa-workflows, blazor-wasm, dotnet-9, workflow-automation, chatbot, semantic-kernel, sql-server, clean-architecture, entity-framework, jwt-auth, pwa, ai-agent
