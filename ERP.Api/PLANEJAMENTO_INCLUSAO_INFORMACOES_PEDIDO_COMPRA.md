# Planejamento: Inclusão de Informações extras na Requisição HTTP do Pedido de Compra

## Objetivo

Adicionar informações complementares no corpo da requisição HTTP enviada ao endpoint `/workflows/aprovacao` nos métodos `Create` e `Update` do `PedidosComprasService`.

## Situação Atual

Os métodos `Create` e `Update` do `PedidosComprasService` enviam um corpo JSON limitado:

```json
{
    "email": "email@exemplo.com",
    "id": "PED-001"
}
```

## Objetivo

Expandir o corpo da requisição para incluir:

- `descricao` - Descrição do pedido
- `quantidade` - Quantidade solicitada
- `valorTotal` - Valor total do pedido (calculado automaticamente)
- `custoMedio` - Custo médio unitário
- `dataPedido` - Data do pedido

### Novo corpo da requisição:

```json
{
    "email": "email@exemplo.com",
    "id": "PED-001",
    "descricao": "Monitor Dell 24 polegadas",
    "quantidade": 10,
    "valorTotal": 4500.00,
    "custoMedio": 450.00,
    "dataPedido": "2026-05-19T10:30:00Z"
}
```

## Arquivo a Ser Modificado

- **Caminho**: `ERP.Application/PedidosComprasService.cs`

## Detalhamento das Alterações

### 1. Método `Create` (linha ~68)

**Código atual:**
```csharp
var body = new { email = pedido.EmailAprovacao, id = pedido.NumeroPedido };
```

**Código alterado:**
```csharp
var body = new {
    email = pedido.EmailAprovacao,
    id = pedido.NumeroPedido,
    descricao = pedido.Descricao,
    quantidade = pedido.Quantidade,
    valorTotal = pedido.ValorTotal,
    custoMedio = pedido.CustoMedio,
    dataPedido = pedido.DataPedido
};
```

### 2. Método `Update` (linha ~121)

**Código atual:**
```csharp
var body = new { email = pedido.EmailAprovacao, id = pedido.NumeroPedido };
```

**Código alterado:**
```csharp
var body = new {
    email = pedido.EmailAprovacao,
    id = pedido.NumeroPedido,
    descricao = pedido.Descricao,
    quantidade = pedido.Quantidade,
    valorTotal = pedido.ValorTotal,
    custoMedio = pedido.CustoMedio,
    dataPedido = pedido.DataPedido
};
```

## Validações Prévias

- ✅ Campo `Descricao` existe na entidade `PedidoCompra` (linha 7)
- ✅ Campo `Quantidade` existe na entidade `PedidoCompra` (linha 10)
- ✅ Campo `CustoMedio` existe na entidade `PedidoCompra` (linha 11)
- ✅ Campo `ValorTotal` existe na entidade `PedidoCompra` (linha 12)
- ✅ Campo `DataPedido` existe na entidade `PedidoCompra` (linha 14)
- ✅ O campo `ValorTotal` já é calculado automaticamente nos métodos Create e Update

## Impacto

- A API do workflow de aprovação receberá informações detalhadas do pedido
- Possibilita criar workflows mais completos e personalizados
- Nãobreaking change - o endpoint anterior continua funcionando (campos adicionais são opcionais)

## Status

- [ ] Pendente de implementação