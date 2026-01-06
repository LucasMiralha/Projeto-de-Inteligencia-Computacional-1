# Agente de IA com busca A* e busca Gulosa com Árvore de Decisão aplicado em Unity

### Vídeo demonstração [aqui](https://youtu.be/P26MvLIDoyo)

Este repositório contém a implementação completa de um sistema de Inteligência Artificial para um agente em Unity. O projeto demonstra a integração de um algoritmo de pathfinding A* com uma Árvore de Decisão (implementada como uma Máquina de Estados Finitos) para criar um agente baseado em utilidade, focado em autopreservação e perseguição de objetivos.

O sistema inclui Gizmos do Unity para exibir em tempo real o processo de busca do algoritmo A*, mostrando os nós explorados, a fronteira de busca e o caminho final.

## Funcionalidades Principais

  Busca A* e Gulosa: Implementação completa do algoritmo A* a partir do zero, e posteriormente do algoritmo de busca gulosa.

  Grid de Navegação Dinâmica: O GridManager gera proceduralmente um grid de nós a partir da geometria de um plano existente na cena, detectando obstáculos com base na LayerMask Obstacle.

  Busca com Tomada de Decisão: A implementação de uma árvore de decisão em formato de FSM cria o modelo baseado em utilidade, fazendo os agentes buscarem preservação além do objetivo final.

  Gestão de Recursos: O agente possui um medidor de vitalidade que decai com o tempo e que o força a procurar "zonas de recuperação" para sobreviver.

  Visualização para Depuração: Utiliza os Gizmos do Unity para desenhar o estado interno dos algoritmos de busca, colorindo os nós do Open Set (borda), Closed Set (explorados) e o Final Path (caminho ótimo).

## Como Funciona

O projeto é construído sobre três pilares principais que trabalham em conjunto.

### 1. Navegação (Buscas A* e Gulosa)

A base do movimento do agente é o nosso sistema de pathfinding customizado.

  GridManager.cs: Discretiza o mundo 3D num grid 2D de Nodes. É responsável por saber quais nós são transitáveis e por traduzir posições do mundo para coordenadas do grid.

  Node.cs: Uma classe de dados que representa cada nó da grid, armazenando os seus custos G, H e F, e a referência ao seu nó anterior para reconstrução do caminho.

  Pathfinder.cs: Contém a implementação das buscas A* e Gulosa que, ao receber um ponto de início e fim, retorna uma lista de nós representando o caminho mais curto.

### 2. Comportamento (Controlador Unificado)

O AgentScript.cs é o componente central que dita as ações do agente.

  Máquina de Estados (FSM): O agente opera em estados (SeekingPrimaryObjective, SeekingRecovery). O método UpdateStateMachine() verifica constantemente as condições do agente para decidir se deve transitar para um novo estado.

  Gestão de Vitalidade: O controlador gere o decaimento e a regeneração da vitalidade. Este recurso é o principal gatilho para as decisões da FSM.

  Orquestração: Quando um estado muda, o controlador solicita um novo caminho ao Pathfinder com base no novo objetivo (o alvo principal ou a zona de recuperação mais próxima). Em seguida, inicia uma Coroutine (FollowPath) para mover o agente ao longo do caminho recebido.

### 3. Visualização (Gizmos)

Para depuração e análise, o GridManager.cs implementa o método OnDrawGizmos(), nativo do Unity.

  Este método obtém as listas de nós openSet, closedSet e finalPath do Pathfinder após cada busca.

  Ele desenha um cubo na posição de cada nó, com uma cor específica que representa o seu estado:

  Vermelho: Nós no Closed Set (já explorados).

  Amarelo: Nós no Open Set (a fronteira de busca atual).

  Verde: Nós que compõem o caminho final ótimo.

## Baseado na imagem:
![deserto](https://github.com/user-attachments/assets/5b6df0df-2776-442a-941b-85f605413a01)
