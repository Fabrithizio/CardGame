# CardGame — Primeiro Set Organizado

## Observações gerais

- Organizei as ideias que você despejou no modelo.
- Mantive sua lógica de raridade por pontos:
  - Comum: até 24 pontos
  - Rara: até 26 pontos
  - Épica: até 27 pontos
  - Lendária: até 28 pontos
- Os 5 primeiros mascotes foram organizados como um grupo de sinergia.
- Imagens com marca d'água visível devem ser usadas apenas no protótipo e trocadas antes de publicar o jogo.

---

## 1. Regras organizadas de atributos secundários

Essas são as regras que eu recomendo como base inicial do protótipo.

### Speed
- Speed 10+: 2 golpes por ataque
- Speed 20+: 3 golpes por ataque

### Defense
- Defense 10+: ganha **Guardião**
- Guardião: reduz em 1 o dano recebido por golpe
- Se começar seu turno sem ter sofrido dano no turno anterior, recupera **Escudo 1**
- Defense 20+: reduz 2 de dano por golpe em vez de 1

### Focus
- Focus 10+: o primeiro golpe que acertar em cada turno vira **Crítico x2**
- Focus 20+: o primeiro golpe que acertar em cada turno vira **Crítico x3**

### Resistance
- Resistance 10+: cura 2 de vida no fim do seu turno
- Resistance 20+: cura 4 de vida no fim do seu turno e remove 1 debuff

> Observação importante:
> esses buffs de atributo afetam a própria carta que tem o atributo.
> Eles não mudam outras cartas diretamente.

---

## 2. Grupo de sinergia: Companhia Aurora

Essas 5 criaturas foram organizadas como um mini-arquétipo.

### Regra compartilhada — Vínculo Aurora
Se você controlar criaturas da **Companhia Aurora** em campo, cada uma ativa níveis extras:

- **2 em campo:** ativa bônus nível 1
- **3 em campo:** ativa bônus nível 2
- **5 em campo:** ativa bônus máximo

As 5 cartas desse grupo são:

1. Lury
2. Luvy
3. Luxya
4. Turga
5. Glovy

---

## 3. Criaturas — Companhia Aurora

## 3.1 Lury
**Arte sugerida:** mascote de fogo / raposinha com velas

```txt
Nome: Lury
Descrição curta: Espírito de fogo veloz que espalha queimadura.
Raridade: Rara
Custo: 3

Atributos: total 26
- Ataque: 4
- Vida: 7
- Speed: 7
- Defense: 2
- Focus: 3
- Resistance: 3

Função no jogo:
Agressiva / pressão inicial.

Efeito/Habilidade:
Ataques de Lury aplicam Queimadura 1.

Queimadura 1:
no começo do turno do dono da criatura afetada, ela sofre 1 de dano.

Vínculo Aurora:
- 2 em campo: Lury ganha +1 Ataque.
- 3 em campo: Lury ganha +2 Speed.
- 5 em campo: o primeiro golpe de cada ataque de Lury causa +1 dano.

Fraqueza/Compensação:
Pouca defesa e vida moderada.

Observações visuais:
Criatura de fogo fofa, brilho quente, ataques flamejantes.
```

---

## 3.2 Luvy
**Arte sugerida:** mascote azul aquático / coelhinha da água

```txt
Nome: Luvy
Descrição curta: Espírito aquático de suporte e proteção.
Raridade: Rara
Custo: 3

Atributos: total 26
- Ataque: 2
- Vida: 8
- Speed: 6
- Defense: 3
- Focus: 3
- Resistance: 4

Função no jogo:
Suporte / cura / proteção.

Efeito/Habilidade:
Ao entrar em campo, cura 2 de vida da criatura aliada mais ferida.

Vínculo Aurora:
- 2 em campo: no fim do seu turno, cura 1 de vida de um aliado.
- 3 em campo: criaturas Aurora aliadas ganham +1 Resistance.
- 5 em campo: no começo do seu turno, concede Escudo 1 ao aliado com menos vida.

Fraqueza/Compensação:
Dano baixo. Não finaliza partidas sozinha.

Observações visuais:
Água, lua, bolhas, lago noturno.
```

---

## 3.3 Luxya
**Arte sugerida:** mascote clara / raposinha de estrelas e luz

```txt
Nome: Luxya
Descrição curta: Espírito estelar focado em precisão e críticos.
Raridade: Épica
Custo: 4

Atributos: total 27
- Ataque: 3
- Vida: 8
- Speed: 6
- Defense: 3
- Focus: 5
- Resistance: 2

Função no jogo:
Suporte ofensiva / precisão / crítico.

Efeito/Habilidade:
O primeiro golpe que Luxya acertar em cada turno recebe +1 dano.

Vínculo Aurora:
- 2 em campo: Luxya ganha +2 Focus.
- 3 em campo: Luxya pode atingir criaturas com Furtivo.
- 5 em campo: o primeiro golpe de Luxya em cada turno vira Crítico x2 e cura 2 de vida dela.

Fraqueza/Compensação:
Não tem muito ataque bruto nem defesa alta.

Observações visuais:
Luz suave, estrelas, brilho dourado-esverdeado.
```

---

## 3.4 Turga
**Arte sugerida:** tartaruguinha de musgo / guardião da floresta

```txt
Nome: Turga
Descrição curta: Guardião natural focado em defesa e proteção.
Raridade: Rara
Custo: 3

Atributos: total 26
- Ataque: 2
- Vida: 10
- Speed: 3
- Defense: 6
- Focus: 1
- Resistance: 4

Função no jogo:
Tanque / defesa / proteção de campo.

Efeito/Habilidade:
Turga entra em campo com Provocar.

Vínculo Aurora:
- 2 em campo: Turga ganha +1 Defense.
- 3 em campo: Turga reduz 1 dano extra por golpe recebido.
- 5 em campo: se Turga sobreviver ao turno inimigo, recupera Escudo 1 no começo do seu turno.

Fraqueza/Compensação:
Muito lento, pouca pressão ofensiva.

Observações visuais:
Musgo, cogumelos, cristais, floresta viva.
```

---

## 3.5 Glovy
**Arte sugerida:** mascote sombria / familiar do eclipse roxo

```txt
Nome: Glovy
Descrição curta: Espírito sombrío que atrapalha o inimigo.
Raridade: Épica
Custo: 4

Atributos: total 27
- Ataque: 4
- Vida: 7
- Speed: 6
- Defense: 2
- Focus: 4
- Resistance: 4

Função no jogo:
Controle / debuff / interrupção.

Efeito/Habilidade:
Quando Glovy causar dano a uma criatura, ela perde 2 Speed até o fim do próximo turno do dono dela.

Vínculo Aurora:
- 2 em campo: Glovy ganha +1 Focus.
- 3 em campo: criaturas atingidas por Glovy não podem ativar sua habilidade até o fim do próximo turno.
- 5 em campo: a primeira criatura atingida por Glovy em cada turno fica Silenciada por 1 turno.

Fraqueza/Compensação:
Não é muito resistente.

Observações visuais:
Lua roxa, cristais escuros, borboletas sombrias.
```

---

## 4. Criaturas extras

## 4.1 Cavaleiro Rubro dos Estilhaços
**Arte sugerida:** cavaleiro com cristais vermelhos

```txt
Nome: Cavaleiro Rubro dos Estilhaços
Descrição curta: guerreiro agressivo que atravessa defesas.
Raridade: Épica
Custo: 5

Atributos: total 27
- Ataque: 6
- Vida: 8
- Speed: 4
- Defense: 4
- Focus: 3
- Resistance: 2

Função no jogo:
Finalizador / ofensiva pesada.

Efeito/Habilidade:
Se causar dano a uma criatura com Escudo, quebra o Escudo e ainda causa 1 dano extra.

Habilidade adicional:
Ao entrar em campo, causa 2 de dano a uma criatura inimiga.

Fraqueza/Compensação:
Custo alto e resistência baixa para um atacante pesado.
```

---

## 4.2 Serpente do Trovão Azure
**Arte sugerida:** dragão azul elétrico

```txt
Nome: Serpente do Trovão Azure
Descrição curta: dragão elétrico rápido e ameaçador.
Raridade: Lendária
Custo: 6

Atributos: total 28
- Ataque: 7
- Vida: 8
- Speed: 8
- Defense: 2
- Focus: 2
- Resistance: 1

Função no jogo:
Pressão / dano explosivo.

Efeito/Habilidade:
Quando ataca, causa 1 de dano elétrico adicional ao alvo antes do combate.

Condição:
Se atingir Speed 10+, também pode escolher atacar diretamente o jogador inimigo.

Fraqueza/Compensação:
Pouca defesa e pouca resistência para o custo.
```

---

## 4.3 Dragão Astral de Cristal
**Arte sugerida:** dragão cósmico de cristais azuis

```txt
Nome: Dragão Astral de Cristal
Descrição curta: criatura lendária de domínio mágico e presença de campo.
Raridade: Lendária
Custo: 7

Atributos: total 28
- Ataque: 6
- Vida: 9
- Speed: 5
- Defense: 3
- Focus: 3
- Resistance: 2

Função no jogo:
Chefe de fim de jogo / presença dominante.

Efeito/Habilidade:
Ao entrar em campo, ganha Escudo 1.

Habilidade adicional:
No fim do seu turno, se o Dragão Astral estiver em campo, ganha +1 Focus até o máximo de +3 acumulado nesta partida.

Fraqueza/Compensação:
Muito caro. Entra tarde no jogo.
```

---

## 4.4 Colosso Rúnico Dourado
**Arte sugerida:** golem de pedra com runas douradas
**Observação:** imagem com marca d'água, usar no protótipo apenas.

```txt
Nome: Colosso Rúnico Dourado
Descrição curta: gigante de pedra que segura a linha de frente.
Raridade: Épica
Custo: 5

Atributos: total 27
- Ataque: 5
- Vida: 10
- Speed: 2
- Defense: 6
- Focus: 1
- Resistance: 3

Função no jogo:
Tanque / defensor principal.

Efeito/Habilidade:
Ao entrar em campo, ganha Escudo 1.

Habilidade adicional:
Enquanto estiver em campo, o primeiro ataque inimigo de cada turno deve escolher o Colosso se ele estiver disponível.

Fraqueza/Compensação:
Muito lento.
```

---

## 4.5 Ancião do Brejo Luminoso
**Arte sugerida:** golem de pântano com musgo e cogumelos

```txt
Nome: Ancião do Brejo Luminoso
Descrição curta: guardião vivo do pântano, resistente e regenerativo.
Raridade: Épica
Custo: 5

Atributos: total 27
- Ataque: 4
- Vida: 10
- Speed: 3
- Defense: 5
- Focus: 1
- Resistance: 4

Função no jogo:
Tanque regenerativo / anti-desgaste.

Efeito/Habilidade:
No fim do seu turno, recupera 2 de vida.

Habilidade adicional:
Se estiver com Resistance 10+ por buff, também remove 1 debuff de si mesmo no fim do turno.

Fraqueza/Compensação:
Dano baixo.
```

---

## 5. Magias

## 5.1 Aprisionamento da Alma
**Arte sugerida:** cavaleiro preso por correntes roxas

```txt
Nome: Aprisionamento da Alma
Descrição curta: correntes arcanas impedem a ação de um alvo.
Raridade: Comum
Custo: 2

Categoria:
Magia de Ação

Momento de uso:
No seu turno, fase principal.

Precisa de alvo?
Sim

Tipo de alvo:
1 criatura inimiga em campo.

Efeito:
A criatura alvo não pode atacar por 2 turnos.

Duração:
2 turnos do dono da criatura afetada.

Condição:
A criatura precisa estar em campo.

Fraqueza/Compensação:
Não remove a criatura, só trava o ataque.
```

---

## 5.2 Orbe Ígneo
**Arte sugerida:** conjurador segurando uma bola de fogo
**Observação:** imagem com marca d'água, usar no protótipo apenas.

```txt
Nome: Orbe Ígneo
Descrição curta: magia rápida de dano direto.
Raridade: Comum
Custo: 1

Categoria:
Magia Rápida

Momento de uso:
No seu turno ou como resposta ao declarar ataque.

Precisa de alvo?
Sim

Tipo de alvo:
1 criatura em campo.

Efeito:
Causa 2 de dano ao alvo.

Duração:
Imediato.

Fraqueza/Compensação:
Dano baixo, serve mais para finalizar ou quebrar escudo.
```

---

## 5.3 Florescer da Loucura
**Arte sugerida:** flor roxa hipnótica com criaturas afetadas

```txt
Nome: Florescer da Loucura
Descrição curta: flor arcana enlouquece o campo inimigo.
Raridade: Rara
Custo: 3

Categoria:
Magia de Ação

Momento de uso:
No seu turno.

Precisa de alvo?
Sim

Tipo de alvo:
1 criatura inimiga.

Efeito:
A criatura alvo perde 3 Speed e 2 Focus até o fim do próximo turno do dono dela.

Duração:
Até o fim do próximo turno do dono da criatura.

Fraqueza/Compensação:
Não causa dano direto.
```

---

## 5.4 Raio de Ruptura
**Arte sugerida:** mão conjurando um relâmpago gigante no campo

```txt
Nome: Raio de Ruptura
Descrição curta: explosão elétrica pesada.
Raridade: Épica
Custo: 4

Categoria:
Magia de Ação

Momento de uso:
No seu turno.

Precisa de alvo?
Sim

Tipo de alvo:
1 criatura inimiga ou o jogador inimigo, se ele não tiver criaturas em campo.

Efeito:
Causa 4 de dano ao alvo.

Duração:
Imediato.

Fraqueza/Compensação:
Custo moderado e foco em alvo único.
```

---

## 6. Armadilhas

## 6.1 Poço de Estacas
**Arte sugerida:** buraco com espinhos e energia elétrica
**Observação:** imagem com marca d'água, usar no protótipo apenas.

```txt
Nome: Poço de Estacas
Descrição curta: armadilha brutal que pune um ataque.
Raridade: Rara
Custo: 2

Categoria:
Armadilha de Punição

Gatilho:
Quando uma criatura inimiga declarar ataque.

Precisa de alvo?
Não

Tipo de alvo:
A criatura atacante.

Efeito:
A criatura atacante sofre 3 de dano e perde 2 Speed. se ela ia atacar 2 vezes por que tinha 10 de speed nao via mais porque perdeu 2 

Duração:
Imediato + debuff até fim do turno.

Condição:
A armadilha precisa estar armada no campo.

A armadilha é revelada quando?
Ao ativar.

Fraqueza/Compensação:
Só funciona uma vez.
```

---

## 7. Arenas

## 7.1 Paz Celestial
**Arte sugerida:** lugar calmo, luz suave, ambiente sereno

```txt
Nome: Paz Celestial
Descrição curta: uma arena pacífica que suprime despertares de atributos.
Raridade: Épica
Custo: 1

Tipo:
Arena permanente até ser substituída ou destruída.

Efeito global:
Nenhum atributo ativa seus buffs de tier enquanto esta Arena estiver ativa.
Exemplo:
- Speed 10+ não gera golpes extras
- Defense 10+ não ativa Guardião
- Focus 10+ não ativa Crítico
- Resistance 10+ não ativa cura

Afeta:
Ambos os jogadores.

Duração:
Até outra Arena entrar ou esta ser destruída.

Condição:
Só pode existir 1 Arena ativa por vez.
```

---

## 7.2 Portal Prismático
**Arte sugerida:** templo com vórtice de luz e cristais

```txt
Nome: Portal Prismático
Descrição curta: campo energético que favorece criaturas mais místicas.
Raridade: Épica
Custo: 2

Tipo:
Arena permanente até ser substituída ou destruída.

Efeito global:
Todas as criaturas em campo ganham +1 Focus e +1 Resistance.

Afeta:
Ambos os jogadores.

Duração:
Até outra Arena entrar ou esta ser destruída.

Condição:
Só pode existir 1 Arena ativa por vez.
```

---

## 8. Míticos

## 8.1 Ruptura Temporal
**Arte sugerida:** vórtice roxo de tempo e relógios

```txt
Nome: Ruptura Temporal
Descrição curta: dobra o fluxo da partida e quebra o ritmo do inimigo.
Raridade/Classe: Mítico
Quantidade permitida:
2 por partida

Categoria:
Mítico de Controle

Momento de uso:
A qualquer momento.

Precisa de alvo?
Não

Tipo de alvo:
Campo inteiro.

Efeito:
Encerra imediatamente a fase atual e impede o inimigo de entrar na fase de Batalha no próximo turno dele.

Duração:
Até o próximo turno do inimigo.

Tem punição por usar?
Em teste

Punição possível:
Você compra 1 carta a menos no seu próximo turno.

Impacto desejado:
Salvar uma situação ruim ou travar uma ofensiva decisiva.
```

---

## 8.2 Juramento Celestial
**Arte sugerida:** figura angelical com círculo sagrado de luz

```txt
Nome: Juramento Celestial
Descrição curta: bênção extrema para estabilizar a partida.
Raridade/Classe: Mítico
Quantidade permitida:
2 por partida

Categoria:
Mítico de Defesa

Momento de uso:
A qualquer momento.

Precisa de alvo?
Não

Tipo de alvo:
Seu campo e seu jogador.

Efeito:
Cura 4 de vida do jogador e concede Escudo 1 a todas as criaturas aliadas em campo.

Duração:
Imediato + escudos ativos até serem consumidos.

Tem punição por usar?
Em teste

Punição possível:
No seu próximo turno, você perde 1 de energia máxima temporariamente.

Impacto desejado:
Sobrevivência e virada defensiva.
```

---

## 8.3 Tempestade Violeta do Fim
**Arte sugerida:** céu de relâmpagos roxos devastando o campo

```txt
Nome: Tempestade Violeta do Fim
Descrição curta: cataclismo que atinge toda a mesa.
Raridade/Classe: Mítico
Quantidade permitida:
2 por partida

Categoria:
Mítico Ofensivo

Momento de uso:
A qualquer momento.

Precisa de alvo?
Não

Tipo de alvo:
Campo inteiro.

Efeito:
Causa 2 de dano a todas as criaturas em campo e 2 de dano ao jogador inimigo.

Duração:
Imediato.

Tem punição por usar?
Em teste

Punição possível:
Suas criaturas também sofrem 1 de dano adicional após a resolução.

Impacto desejado:
Virada agressiva / limpeza parcial do campo.
```

---

## 9. Observações de design

### O que eu preservei das suas ideias
- Míticos como “magia única” fora do deck.
- Possibilidade de punição por usar Mítico.
- Tipos separados de magia, armadilha e arena.
- Atributos secundários servindo para melhorar a própria carta.
- Grupo de 5 criaturas com sinergia progressiva.
- Cartas montadas pela UI do Unity, não como imagem pronta com texto fixo.

### O que eu refinei
- Dei função real a cada imagem.
- Balanceei custos e atributos para o protótipo.
- Transformei suas ideias vagas em efeitos jogáveis.
- Organizei um primeiro mini-set pronto para testar.

---

## 10. Próximo passo recomendado

1. Primeiro, escolher quais dessas cartas entram no **primeiro deck de teste**.
2. Depois, eu monto a próxima lista em formato mais técnico, pronta para virar:
   - ScriptableObject
   - enum/tipos
   - efeitos no Unity

### Meu palpite de primeiro pacote para testar no jogo
- Lury
- Luvy
- Turga
- Glovy
- Cavaleiro Rubro dos Estilhaços
- Aprisionamento da Alma
- Orbe Ígneo
- Poço de Estacas
- Paz Celestial
- Ruptura Temporal
