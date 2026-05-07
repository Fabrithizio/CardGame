# CardGame — Deck Inicial de Teste 01

Este deck foi montado para testar as principais mecânicas atuais do protótipo:

- campo com criaturas;
- energia;
- compra automática;
- múltiplos golpes por Speed;
- armadilhas;
- magias;
- arena;
- sinergia entre criaturas;
- Míticos como recurso fora do deck.

---

## 1. Configuração da partida

```txt
Vida inicial do jogador: 30
Vida inicial do inimigo: 30
Cartas iniciais na mão: 5
Míticos por jogador: 2
Tamanho do deck de teste: 20 cartas
```

---

## 2. Míticos do jogador

Míticos não entram no deck.

Use estes 2 no loadout do jogador:

## 2.1 Juramento Celestial

```txt
Tipo: Mítico
Categoria: Mítico de Defesa

Efeito:
Cura 4 de vida do jogador e concede Escudo 1 para todas as criaturas aliadas em campo.

Momento:
A qualquer momento.

Punição futura em teste:
No próximo turno, o jogador perde 1 de energia máxima temporariamente.

Função:
Salvar o jogador em momento crítico.
```

## 2.2 Ruptura Temporal

```txt
Tipo: Mítico
Categoria: Mítico de Controle

Efeito:
Impede o inimigo de entrar na fase de Batalha no próximo turno dele.

Momento:
A qualquer momento.

Punição futura em teste:
O jogador compra 1 carta a menos no próximo turno.

Função:
Parar um ataque decisivo.
```

---

## 3. Deck inicial do jogador — 20 cartas

## Criaturas — 12 cartas

```txt
3x Lury
3x Luvy
2x Turga
2x Glovy
1x Cavaleiro Rubro dos Estilhaços
1x Colosso Rúnico Dourado
```

## Magias — 5 cartas

```txt
2x Orbe Ígneo
1x Aprisionamento da Alma
1x Florescer da Loucura
1x Raio de Ruptura
```

## Armadilhas — 2 cartas

```txt
2x Poço de Estacas
```

## Arena — 1 carta

```txt
1x Paz Celestial
```

Total:

```txt
12 criaturas
5 magias
2 armadilhas
1 arena
= 20 cartas
```

---

## 4. Deck inicial do inimigo — 20 cartas

Para testar de forma equilibrada, use uma versão parecida, mas um pouco mais agressiva:

## Criaturas — 13 cartas

```txt
4x Lury
3x Turga
2x Glovy
2x Cavaleiro Rubro dos Estilhaços
1x Colosso Rúnico Dourado
1x Ancião do Brejo Luminoso
```

## Magias — 4 cartas

```txt
2x Orbe Ígneo
1x Florescer da Loucura
1x Raio de Ruptura
```

## Armadilhas — 2 cartas

```txt
2x Poço de Estacas
```

## Arena — 1 carta

```txt
1x Paz Celestial
```

Total:

```txt
13 criaturas
4 magias
2 armadilhas
1 arena
= 20 cartas
```

---

## 5. Lista técnica das cartas

## 5.1 Lury

```txt
Tipo: Creature
Raridade: Rare
Custo: 3

Ataque: 4
Vida: 7
Speed: 7
Defense: 2
Focus: 3
Resistance: 3

Efeito:
Ao causar dano em uma criatura, aplica Queimadura 1.

Queimadura 1:
No começo do turno do dono da criatura afetada, ela sofre 1 de dano.
```

## 5.2 Luvy

```txt
Tipo: Creature
Raridade: Rare
Custo: 3

Ataque: 2
Vida: 8
Speed: 6
Defense: 3
Focus: 3
Resistance: 4

Efeito:
Ao entrar em campo, cura 2 de vida da criatura aliada mais ferida.
```

## 5.3 Turga

```txt
Tipo: Creature
Raridade: Rare
Custo: 3

Ataque: 2
Vida: 10
Speed: 3
Defense: 6
Focus: 1
Resistance: 4

Efeito:
Entra em campo com Provocar.
```

## 5.4 Glovy

```txt
Tipo: Creature
Raridade: Epic
Custo: 4

Ataque: 4
Vida: 7
Speed: 6
Defense: 2
Focus: 4
Resistance: 4

Efeito:
Quando causa dano a uma criatura, ela perde 2 Speed até o fim do próximo turno do dono dela.
```

## 5.5 Cavaleiro Rubro dos Estilhaços

```txt
Tipo: Creature
Raridade: Epic
Custo: 5

Ataque: 6
Vida: 8
Speed: 4
Defense: 4
Focus: 3
Resistance: 2

Efeito:
Ao entrar em campo, causa 2 de dano a uma criatura inimiga.
Se atingir uma criatura com Escudo, quebra o Escudo e causa +1 dano.
```

## 5.6 Colosso Rúnico Dourado

```txt
Tipo: Creature
Raridade: Epic
Custo: 5

Ataque: 5
Vida: 10
Speed: 2
Defense: 6
Focus: 1
Resistance: 3

Efeito:
Ao entrar em campo, ganha Escudo 1.
Enquanto estiver em campo, o primeiro ataque inimigo do turno deve escolher o Colosso se ele estiver disponível.
```

## 5.7 Ancião do Brejo Luminoso

```txt
Tipo: Creature
Raridade: Epic
Custo: 5

Ataque: 4
Vida: 10
Speed: 3
Defense: 5
Focus: 1
Resistance: 4

Efeito:
No fim do seu turno, recupera 2 de vida.
```

## 5.8 Orbe Ígneo

```txt
Tipo: Spell
Categoria: Magia Rápida
Raridade: Common
Custo: 1

Efeito:
Causa 2 de dano a 1 criatura em campo.
```

## 5.9 Aprisionamento da Alma

```txt
Tipo: Spell
Categoria: Magia de Ação
Raridade: Common
Custo: 2

Efeito:
A criatura inimiga alvo não pode atacar por 2 turnos.
```

## 5.10 Florescer da Loucura

```txt
Tipo: Spell
Categoria: Magia de Ação
Raridade: Rare
Custo: 3

Efeito:
A criatura inimiga alvo perde 3 Speed e 2 Focus até o fim do próximo turno do dono dela.
```

## 5.11 Raio de Ruptura

```txt
Tipo: Spell
Categoria: Magia de Ação
Raridade: Epic
Custo: 4

Efeito:
Causa 4 de dano a 1 criatura inimiga.
Se o inimigo não tiver criaturas em campo, pode causar 4 de dano ao jogador inimigo.
```

## 5.12 Poço de Estacas

```txt
Tipo: Trap
Categoria: Armadilha de Punição
Raridade: Rare
Custo: 2

Gatilho:
Quando uma criatura inimiga declarar ataque.

Efeito:
A criatura atacante sofre 3 de dano e perde 2 Speed até o fim do turno.
A armadilha é consumida.
```

## 5.13 Paz Celestial

```txt
Tipo: Arena
Raridade: Epic
Custo: 1

Efeito:
Enquanto estiver ativa, atributos não ativam buffs de tier.

Exemplo:
Speed 10+ não dá golpes extras.
Defense 10+ não ativa Guardião.
Focus 10+ não ativa Crítico.
Resistance 10+ não ativa cura.

Regra:
Só pode existir 1 Arena ativa.
```

---

## 6. O que ainda não está implementado no código atual

Estas cartas já estão desenhadas como design, mas algumas mecânicas ainda precisam ser codadas:

```txt
- Queimadura
- Provocar completo
- Debuff temporário
- Magias jogáveis com alvo
- Arena ativa
- Sinergia Companhia Aurora
- Crítico por Focus
- Cura por Resistance
- Guardião por Defense
- Poço de Estacas completo
```

Para o protótipo atual, as cartas podem ser criadas como dados primeiro. Os efeitos avançados entram depois no sistema genérico de efeitos.

---

## 7. Ordem recomendada de implementação

```txt
1. Criar ScriptableObjects dessas cartas.
2. Colocar o deck de 20 cartas no BattleManager.
3. Implementar magia simples com alvo.
4. Implementar efeitos temporários.
5. Implementar Queimadura.
6. Implementar Provocar.
7. Implementar Arena.
8. Implementar sinergia Companhia Aurora.
```
