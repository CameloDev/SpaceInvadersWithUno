# Space Invaders com Uno Platform

![Space Invaders](https://img.shields.io/badge/Game-Space%20Invaders-blueviolet) ![Uno Platform](https://img.shields.io/badge/Platform-Uno%20Platform-0078D7) ![C#](https://img.shields.io/badge/Language-C%23-239120)

## 🎮 Demonstração do Jogo

### Funcionalidades Implementadas

**Tela Inicial e Navegação:**
- Menu principal com opções para iniciar jogo, ver placares e controles
- Interface intuitiva e responsiva

**Sistema de Jogabilidade:**
- Movimentação suave da nave do jogador com teclas direcionais
- Sistema de tiro com limitação de um projétil por vez (tecla Espaço)
- Padrão de movimento dos inimigos: lateral com descida progressiva
- Nave OVNI especial que aparece periodicamente com pontuação aleatória

**Sistema de Defesa:**
- Barreiras destrutíveis que sofrem dano realista por pixel
- Visualização do desgaste das barreiras com mudança de aparência
- Remoção automática quando completamente destruídas

**Sistema de Progressão:**
- Pontuação em tempo real no canto superior
- Múltiplas ondas com dificuldade progressiva
- Sistema de vidas extras (1 vida a cada 1000 pontos, máximo de 6 vidas)

**Condições de Término:**
- Game over por perda de todas as vidas
- Game over por invasão inimiga (chegada ao nível do jogador)

**Recursos Audiovisuais:**
- Efeitos sonoros para tiros, explosões e eventos especiais
- Trilha sonora ambiente durante o jogo
- Efeitos visuais de explosão e partículas estelares

**Persistência de Dados:**
- Sistema de placares com salvamento em arquivo texto
- Recuperação de pontuações anteriores

## 🏗️ Arquitetura e Código

### Estrutura do Projeto

```
SpaceInvadersWithUno/
├── Controllers/ (Gerenciadores)
│   ├── GerenciadorJogo.cs
│   ├── GerenciadorJogador.cs
│   ├── GerenciadorInimigos.cs
│   ├── GerenciadorProjeteis.cs
│   ├── GerenciadorParticulas.cs
│   └── GerenciadorBarreiras.cs
├── Models/
│   ├── Barreira.cs
│   ├── Inimigo.cs
│   ├── Jogador.cs
│   ├── Projetil.cs
│   └── ToView/
│       └── Particulas.cs
├── Views/
│   ├── PaginaJogo.xaml
│   ├── PaginaJogo.xaml.cs
│   ├── MainPage.xaml
│   └── MainPage.xaml.cs
├── Assets/
│   └── (Recursos visuais e sonoros)
└── Utils/
    ├── LoopStream.cs 
    └── SoundPlayer.cs
```

### Padrão MVVM e Separação de Responsabilidades

O projeto segue uma arquitetura modular com separação clara de concerns:

**Model (Models/):**
- Classes de dados representando entidades do jogo
- Propriedades e estado dos objetos

**View (Views/):**
- Interfaces XAML para renderização
- Binding com ViewModels através de code-behind

**Controller/ViewModel (Controllers/):**
- Lógica de negócio e controle do jogo
- Gerenciadores especializados para cada aspecto

### Gerenciadores Principais

**GerenciadorJogo:**
- Coordenação geral do fluxo do jogo
- Controle de estado e transições
- Verificação de condições de vitória/derrota
- Gerenciamento de pontuação

**GerenciadorJogador:**
- Controle de movimento e input
- Sistema de vidas e vidas extras
- Gerenciamento de tiros do jogador

**GerenciadorInimigos:**
- Geração de ondas de inimigos
- Movimentação coordenada
- Spawn e comportamento do OVNI
- Sistema de tiros inimigos

**GerenciadorProjeteis:**
- Controle de todos os projéteis em cena
- Detecção de colisões
- Remoção de projéteis fora de cena

**GerenciadorBarreiras:**
- Criação e posicionamento de barreiras
- Sistema de dano por pixel
- Renderização com WriteableBitmap

### Manipulação de Eventos

**Input do Teclado:**
```csharp
// Captura de eventos KeyDown e KeyUp
this.KeyDown += Pagina_TeclaPressionada;
this.KeyUp += Pagina_TeclaLiberada;
```

**Sistema de Colisões:**
- Verificação por interseção de retângulos
- Algoritmo otimizado para performance
- Dano preciso nas barreiras usando coordenadas de pixel

### Controle de Timing e Animação

**DispatcherTimer para Game Loop:**
```csharp
_timerJogo = new DispatcherTimer();
_timerJogo.Interval = TimeSpan.FromMilliseconds(16); // ~60 FPS
_timerJogo.Tick += (s, e) => _gerenciadorJogo.LoopJogo();
```

**Timer para Eventos Especiais:**
- Spawn periódico do OVNI
- Animação de partículas de fundo

### Sistema de Pontuação e Vidas

**Atribuição de Pontos:**
- Inimigos superiores: 50 pontos
- Inimigos intermediários: 20 pontos  
- Inimigos inferiores: 10 pontos
- OVNI: 100-300 pontos (aleatório)

**Sistema de Vidas Extras:**
```csharp
public void VerificarVidaExtra(int pontuacao)
{
    int vidasGanhas = pontuacao / 1000;
    if (vidasGanhas > _vidasExtrasConcedidas && Jogador.Vidas < 6)
    {
        Jogador.Vidas++;
        _vidasExtrasConcedidas++;
    }
}
```

### Persistência de Dados

**Sistema de Arquivos:**
- Leitura/escrita de placares em arquivo texto
- Formato simples para fácil manipulação
- Persistência entre sessões de jogo

## 📋 Documentação

### Manual do Jogador

**Como Jogar:**
- **Setas Esquerda/Direita**: Movimentar a nave
- **Barra de Espaço**: Atirar (1 tiro por vez)
- **Objetivo**: Destruir todas as naves inimigas

**Regras do Jogo:**
- Você inicia com 3 vidas
- Ganha 1 vida extra a cada 1000 pontos
- Máximo de 6 vidas
- Game over se perder todas as vidas ou inimigos chegarem à sua altura

**Pontuação:**
- Inimigos superiores: 50 pontos
- Inimigos intermediários: 20 pontos
- Inimigos inferiores: 10 pontos
- OVNI: 100-300 pontos aleatórios

### Fluxo do Jogo

1. **Tela Inicial** → Seleção de opções
2. **Jogo Principal** → Gameplay com múltiplas ondas
3. **Game Over** → Exibição de pontuação final
4. **Placar** → Registro e visualização de scores

### Requisitos do Sistema

- **Sistema Operacional**: Windows 10/11, Android, ou outros suportados por Uno Platform
- **.NET**: Versão 6.0 ou superior
- **Espaço em Disco**: ~50 MB
- **Input**: Teclado para controle

### Testes e Validação

**Casos de Teste Implementados:**
- Movimentação do jogador dentro dos limites da tela
- Sistema de colisão projétil-inimigo
- Dano progressivo nas barreiras
- Spawn e movimento do OVNI
- Sistema de pontuação e vidas extras
- Persistência de placares

**Validação de Funcionamento:**
- Todas as teclas respondem corretamente
- Colisões são detectadas com precisão
- Pontuação é calculada adequadamente
- Transições entre telas são suaves
- Sons são reproduzidos nos eventos corretos

## ❓ Questionamentos do Professor

### 1. Sobre a Arquitetura Escolhida

**Por que usar Gerenciadores em vez de MVVM puro?**
A abordagem com gerenciadores especializados mostrou-se mais eficiente para jogos, onde o estado é complexo e a performance é crítica. Mantivemos a separação de concerns mas com uma estrutura mais orientada a gameplay.

### 2. Performance e Otimização

**Como garantir 60 FPS em dispositivos móveis?**
- Uso de WriteableBitmap para renderização eficiente
- Algoritmos otimizados de colisão
- Controle preciso do game loop com DispatcherTimer
- Limpeza agressiva de objetos não visíveis

### 3. Persistência de Dados

**Por que arquivo texto em vez de banco de dados?**
Para placares simples, um arquivo texto oferece simplicidade e portabilidade sem dependências externas, sendo suficiente para as necessidades do jogo.

### 4. Cross-Platform com Uno Platform

**Quais desafios encontrados no desenvolvimento multiplataforma?**
- Adaptação de input para diferentes dispositivos
- Otimização de performance para hardware variado
- Garantia de consistência visual entre plataformas

### 5. Sistema de Colisão

**Por que a escolha do algoritmo de interseção de retângulos?**
Oferece bom equilíbrio entre precisão e performance, sendo suficiente para a jogabilidade proposta e eficiente computationalmente.

### 6. Expansibilidade

**Como o código permite adicionar novos features?**
A arquitetura modular permite fácil extensão:
- Novos tipos de inimigos podem ser adicionados no GerenciadorInimigos
- Novos power-ups podem ser implementados como sistemas independentes
- Diferentes fases podem ser configuradas através de parâmetros

---

**Desenvolvido com Uno Platform** - Uma experiência clássica de arcade reinventada para plataformas modernas! 🚀👾