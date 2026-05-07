# CardGame — Design Base

## 1. Visão do jogo

CardGame é um jogo de cartas estratégico para mobile, em tela vertical, com foco em adaptação durante a partida.

O jogador monta um deck antes da batalha, mas durante a partida as cartas são compradas de forma aleatória do próprio deck embaralhado. A graça é se adaptar ao que vem na mão, usar energia com inteligência, preparar armadilhas, atacar no momento certo e guardar Míticos para momentos críticos.

A sensação desejada é:

- estratégia de jogo de cartas;
- imprevisibilidade de deck embaralhado;
- campo com criaturas;
- viradas dramáticas com Míticos;
- visual bonito, animado e com impacto;
- inspiração de sensação em jogos como Chaotic e Runeterra, mas com identidade própria.

---

## 2. Estrutura atual da partida

Cada jogador possui:

- até 5 criaturas em campo;
- área separada para armadilhas;
- mão de cartas;
- deck;
- vida;
- energia;
- Míticos fora do deck.

Vitória atual:

- um jogador vence quando a vida do inimigo chega a 0.

Regras futuras a refinar:

- derrota por falta de deck;
- dano de fadiga ao tentar comprar sem cartas;
- condição de derrota se não tiver deck, campo nem reação possível.

---

## 3. Deck e compra

O deck é definido antes da partida.

Durante a batalha:

- o deck é embaralhado;
- o jogador compra cartas automaticamente no começo do próprio turno;
- o jogador não escolhe qualquer carta livremente;
- a aleatoriedade do deck força adaptação.

---

## 4. Energia

A energia é o principal limitador de jogadas.

Regra atual:

- no começo do turno, a energia máxima aumenta;
- a energia atual recarrega até o máximo;
- cartas custam energia;
- criaturas podem ser jogadas enquanto houver energia e espaço no campo.

Decisão importante:

- não usar limite fixo de “1 criatura por turno”;
- se o jogador tem energia suficiente, pode baixar várias criaturas baratas;
- se preferir, pode gastar tudo em uma carta mais cara.

---

## 5. Tipos de carta

### Criatura

Cartas que entram no campo e lutam.

Possuem atributos como:

- Ataque;
- Vida;
- Speed;
- Defense;
- Focus;
- Resistance.

### Magia

Cartas de efeito usadas a partir da mão.

Categorias planejadas:

#### Magia Rápida

Pode ser usada em momentos mais flexíveis, talvez até durante o turno inimigo.

Exemplos:

- dar escudo;
- causar dano rápido;
- aumentar Speed;
- impedir ataque direto.

#### Magia de Ação

Usada no próprio turno, geralmente na fase principal.

Exemplos:

- causar dano em uma criatura;
- comprar carta;
- curar criatura;
- fortalecer criatura.

#### Magia de Arena

Afeta o campo ou as regras temporariamente.

Exemplos:

- todos recebem dano no começo do turno;
- ataques diretos ficam bloqueados por 1 turno;
- criaturas rápidas perdem golpe extra temporariamente.

#### Magia Condicional

Só pode ser usada quando uma condição acontece.

Exemplos:

- se uma criatura sua morreu;
- se o inimigo atacou diretamente;
- se você tem menos vida que o inimigo;
- se uma criatura atingiu Speed 10+.

#### Magia de Reação

Responde a ações específicas.

Exemplos:

- responder a ataque;
- responder a Mítico;
- responder a magia inimiga;
- responder a armadilha revelada.

### Armadilha

Carta preparada em campo, fora dos slots de criatura.

Categorias planejadas:

#### Armadilha de Defesa

Protege contra ataque ou dano.

Exemplos:

- bloquear próximo ataque;
- reduzir dano;
- impedir ataque direto.

#### Armadilha de Punição

Faz o inimigo pagar por atacar.

Exemplos:

- causar dano ao atacante;
- reduzir Speed do atacante;
- aplicar debuff.

#### Armadilha de Controle

Altera a jogada do inimigo.

Exemplos:

- cancelar magia;
- devolver criatura para mão;
- travar uma criatura por 1 turno.

#### Armadilha de Sacrifício

Ativa quando algo seu é destruído.

Exemplos:

- quando uma criatura sua morrer, compre 1 carta;
- revive com 1 de vida;
- causa dano ao inimigo.

#### Armadilha de Blefe

Possui condição específica e pode não ativar sempre.

Exemplos:

- só ativa contra ataque direto;
- só ativa contra criatura com Speed 10+;
- só ativa se você tiver menos vida.

### Equipamento

Decisão atual:

- deixar equipamentos para depois;
- por enquanto, buffs permanentes ou temporários serão feitos como magias, armadilhas, Míticos ou efeitos de criaturas.

---

## 6. Míticos

Míticos são poderes especiais fora do deck.

Eles funcionam como uma magia única, sem custo, que pode ser usada a qualquer momento.

Ideia central:

- Mítico é uma salvação ou virada;
- não fica no deck;
- não depende de compra;
- o inimigo não sabe exatamente o que você trouxe;
- deve causar tensão;
- deve ter animação/efeito dramático.

### Quantidade

Decisão atual:

- testar com 2 Míticos por jogador;
- o sistema pode suportar 3, mas o design inicial deve usar 2.

### Custo

Míticos não custam energia.

Porém, ainda está em aberto se haverá punição por uso.

Possíveis punições futuras:

- perder vida ao usar;
- descartar carta;
- perder energia do próximo turno;
- impedir ataque no turno;
- revelar ambos os Míticos;
- reduzir a recompensa final;
- aplicar exaustão em uma criatura.

### Categorias de Mítico

#### Mítico de Defesa

Usado para sobreviver.

Exemplos:

- bloquear todo dano de um ataque;
- impedir dano direto neste turno;
- dar escudo para todas as criaturas.

#### Mítico de Virada

Muda uma situação ruim.

Exemplos:

- reviver uma criatura;
- comprar cartas;
- recuperar vida;
- transformar derrota em chance.

#### Mítico Ofensivo

Cria pressão.

Exemplos:

- dar golpe extra;
- causar dano direto;
- fortalecer todas as criaturas por 1 turno.

#### Mítico de Controle

Altera a mesa.

Exemplos:

- destruir armadilha;
- impedir magias rápidas por 1 turno;
- remover buffs inimigos.

#### Mítico de Campo

Afeta o jogo por um período curto.

Exemplos:

- ninguém pode atacar diretamente por 1 turno;
- todas as criaturas perdem escudo;
- Speed extra não gera golpes neste turno.

---

## 7. Atributos

Atributos não devem ser apenas números. Eles devem criar identidade e estados visíveis na carta.

### Ataque

Define dano físico/base causado por uma criatura.

Possíveis tiers:

- Attack 10+: ameaça ofensiva;
- Attack 20+: dano brutal ou perfuração.

Ideias futuras:

- Attack alto pode desbloquear Perfuração;
- Attack alto pode quebrar escudos com mais facilidade;
- Attack alto pode causar dano excedente ao jogador.

### Vida

Define quanto dano uma criatura aguenta.

Possíveis tiers:

- Health 10+: resistente;
- Health 20+: colosso.

Ideias futuras:

- vida alta pode reduzir efeitos de execução;
- vida alta pode ganhar resistência a destruição direta.

### Speed

Define número de golpes separados.

Regra atual:

- Speed 10+: 2 golpes;
- Speed 20+: 3 golpes.

Importante:

- golpes são separados;
- escudo que bloqueia 1 ataque/golpe é consumido no primeiro golpe;
- golpes seguintes podem acertar.

### Defense

Representa defesa física.

Ideia atual:

- reduz dano recebido de ataques físicos.

Possíveis tiers:

- Defense 10+: ganha estado Guardião;
- Defense 20+: reduz ainda mais dano físico ou protege aliados.

Possíveis efeitos:

- reduzir dano de ataque;
- forçar ataques contra essa criatura;
- bloquear parte do dano direto;
- proteger criatura adjacente no futuro.

### Focus

Precisa ser refinado.

Ideia inicial corrigida:

Focus não deve necessariamente fortalecer magias usadas nessa criatura. Ele deve ser um atributo próprio da criatura.

Possíveis direções:

- resistência a controle mental/efeitos;
- precisão contra furtivos;
- melhora habilidades da própria criatura;
- reduz chance de ser afetada por certas magias;
- permite ativar habilidades especiais.

Possíveis tiers:

- Focus 10+: ignora Furtivo;
- Focus 20+: imune a confusão/controle ou reduz efeitos condicionais.

### Resistance

Representa defesa contra magia/efeitos.

Possíveis tiers:

- Resistance 10+: reduz dano mágico;
- Resistance 20+: ignora ou reduz efeitos negativos.

Possíveis efeitos:

- reduzir dano de magia;
- resistir debuff;
- impedir destruição direta;
- cancelar primeira magia inimiga que afete essa criatura.

---

## 8. Estados e palavras-chave

Estados planejados:

- Shielded: bloqueia próximo golpe/ataque;
- SpeedTier1: Speed 10+, 2 golpes;
- SpeedTier2: Speed 20+, 3 golpes;
- Taunt/Provocar: inimigo deve atacar essa criatura primeiro;
- Stealth/Furtivo: não pode ser alvo por determinado tempo;
- Piercing/Perfuração: dano excedente passa ao jogador;
- Retaliation/Retaliação: causa dano ao atacante;
- LifeSteal/Roubo de vida: cura o jogador ao causar dano;
- Guardião: protege aliados ou força ataques;
- MagicResistant: reduz efeitos mágicos.

---

## 9. Visual e arte

O jogo deve ser feito pensando em mobile vertical.

### Cartas

Não usar a carta inteira como uma imagem com todos os textos fixos.

Melhor estrutura:

- arte da carta separada;
- moldura feita na UI;
- nome renderizado pelo Unity;
- custo renderizado pelo Unity;
- atributos renderizados pelo Unity;
- status/ícones renderizados pelo Unity.

Motivo:

- atributos mudam durante a partida;
- buffs/debuffs aparecem;
- texto pode mudar;
- balanceamento fica mais fácil;
- cartas podem ser atualizadas sem refazer imagem.

### Tamanho recomendado de arte

Para arte da criatura/personagem:

- 1024x1024 para arte quadrada;
- 1024x1536 para arte vertical;
- 768x1024 para arte de carta vertical.

### Tabuleiro

Camadas desejadas:

1. Fundo/arena.
2. Slots de criatura.
3. Slots de armadilha.
4. Cartas.
5. Efeitos visuais.
6. UI principal.
7. Banners dramáticos.

### Visual estilo Runeterra

Não tentar copiar tudo de uma vez.

Caminho recomendado:

1. UI limpa.
2. Cartas com moldura bonita.
3. Animações simples.
4. Efeitos de dano/escudo.
5. Sons fortes.
6. Banners para Míticos.
7. Polimento progressivo.

---

## 10. Animações planejadas

### Entrada de carta no campo

- carta sai da mão;
- cresce um pouco;
- desliza até o slot;
- encaixa com brilho.

### Ataque

- atacante avança;
- alvo treme;
- dano aparece;
- atacante volta.

### Dano

- carta pisca vermelho;
- número sobe;
- som de impacto.

### Escudo

- círculo/barreira aparece;
- ao bloquear, quebra em partículas.

### Morte

- carta treme;
- escurece;
- gira ou dissolve;
- sai do campo.

### Mítico

- tela escurece rapidamente;
- faixa dramática aparece;
- som forte;
- efeito visual no campo/alvo;
- bolinha do Mítico apaga.

---

## 11. Próximas prioridades

Ordem recomendada:

1. Sistema de efeitos genérico.
2. Sistema de alvo para cartas que precisam de alvo.
3. Refinar Míticos como poderes sem custo e de uso livre.
4. Criar categorias reais de magia.
5. Criar mais armadilhas.
6. Criar cartas variadas.
7. Melhorar visual das cartas.
8. Criar animação de ataque/dano/morte.
9. Criar menu inicial.
10. Criar tela de deck antes da partida.
11. Testar no celular Android.

---

## 12. Decisões atuais importantes

- Míticos serão reduzidos para 2 por partida inicialmente.
- Míticos são como magias únicas, sem custo e fora do deck.
- Ainda será avaliada uma punição por usar Mítico.
- Energia é o limite principal para jogar cartas.
- Não haverá limite fixo de 1 criatura por turno.
- Equipamentos ficam para depois.
- Magias e armadilhas serão divididas por categorias.
- Atributos terão tiers e estados próprios.
- Cartas devem ser montadas pela UI do Unity, não como imagem única fixa.
