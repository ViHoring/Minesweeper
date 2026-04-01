# Campo Minado (Unity) — VIVIAN HÖRING

Protótipo de Campo Minado desenvolvido em Unity (Unity 6.x) como parte de um teste técnico para vaga de Game Developer na Golden Treehouse.

## Requisitos
- Unity **6.3 LTS** (ou superior dentro do Unity 6)
- Plataforma: PC Windows — **mouse recomendado**

## Como rodar
1. Clone este repositório
2. Use o branch "main"
3. Abra o projeto na Unity
4. Abra a cena: 'Assets/Scenes/MainScene.unity'
5. Pressione **Play**

## Controles
- **Clique esquerdo:** revela um tile  
- **Clique direito:** marca/desmarca um tile com bandeira  

## Regras (Campo Minado)
- O tabuleiro é uma grid 2D com minas escondidas
- Tiles sem mina mostram um número com a quantidade de minas adjacentes (8 direções)
- Tiles com **0** minas adjacentes revelam área vazia conectada
- Clicar em uma **mina** encerra a partida (derrota)
- Você vence quando:
  - todas as minas estão marcadas e todos os demais tiles foram revelados

## Dificuldades
- **Fácil:** 8x8 / 10 minas  
- **Intermediário:** 16x16 / 40 minas  
- **Difícil:** 30x16 / 99 minas  

## Modos de jogo
- **Clássico:** o tabuleiro pode exigir tentativa (pode haver necessidade de “sorte”)  
- **Sem sorte:** o tabuleiro é gerado para ser resolvível sem chute (modo opcional)

## Extras implementados
- Primeiro clique sempre abre um tile em branco
- HUD (timer e contador de minas marcadas)
- Feedback visual/sonoro (animações de reveal, SFX, etc.)
- Opção **Repetir tabuleiro** (Retry Same Board)

## Organização do código
- GameManager: controle de estados e fluxo do jogo (menu, jogando, pausa, game over)
- BoardController / BoardView: gerenciamento do tabuleiro e atualização visual
- BoardGenerator: geração do board e minas
- TileView / TileInputHandler: interação do jogador e feedback visual

## Observação
- Existe no HUD dois botões para debug, são usados para testar vitória ou derrota à partir
do momento que temos um board gerado e pelo menos uma flag usada. Mantive os botões desativados,
mas podem ser usados para teste caso tenha o interesse.


## Créditos
- Sons: Creative Commons Zero, CC0 - Created/distributed by Kenney (www.kenney.nl)
  http://creativecommons.org/publicdomain/zero/1.0/