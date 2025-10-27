name: Unity WebGL Build and Deploy

on:
  push:
    branches:
      - main
  workflow_dispatch:

  # 権限を追加
permissions:
  contents: write
  pages: write
  id-token: write

env:
  UNITY_VERSION: 6000.2.2f1

jobs:
  build:
    name: Build WebGL
    runs-on: ubuntu-latest
    
    steps:
      # リポジトリのチェックアウト
      - name: Checkout repository
        uses: actions/checkout@v4
        with:
          lfs: true
          fetch-depth: 0

      # LFSファイルのキャッシュ
      - name: Cache LFS
        uses: actions/cache@v4
        with:
          path: .git/lfs
          key: lfs-${{ hashFiles('.lfs-assets-id') }}
          restore-keys: |
            lfs-
      # Libraryフォルダのキャッシュ（ビルド高速化）
      - name: Cache Library
        uses: actions/cache@v4
        with:
          path: Library
          key: Library-WebGL-6000.2.2f1-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
          restore-keys: |
            Library-WebGL-6000.2.2f1-
            Library-WebGL-
            Library-
      # ディスク容量の確保
      - name: Free Disk Space
        run: |
          sudo rm -rf /usr/share/dotnet
          sudo rm -rf /opt/ghc
          sudo rm -rf "/usr/local/share/boost"
          sudo apt-get clean
          echo "=== Disk space after cleanup ==="
          df -h
      # WebGLBuilder.csが存在するか確認
      - name: Verify Build Script
        run: |
          echo "=== Checking for WebGLBuilder.cs ==="
          if [ -f "Assets/Editor/WebGLBuilder.cs" ]; then
            echo "✓ WebGLBuilder.cs found"
            cat Assets/Editor/WebGLBuilder.cs
          else
            echo "✗ WebGLBuilder.cs not found!"
            echo "Available files in Assets/Editor:"
            ls -la Assets/Editor/ || echo "Assets/Editor/ directory not found"
          fi
      # GameCIを使用してWebGLビルド
      - name: Build WebGL
        uses: game-ci/unity-builder@v4
        timeout-minutes: 60
        env:
          UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}
          UNITY_EMAIL: ${{ secrets.UNITY_EMAIL }}
          UNITY_PASSWORD: ${{ secrets.UNITY_PASSWORD }}
        with:
          targetPlatform: WebGL
          unityVersion: ${{ env.UNITY_VERSION }}
          buildMethod: WebGLBuilder.BuildWebGL
          allowDirtyBuild: true

      # ビルド結果の確認
      - name: Check Build Output
        if: always()
        run: |
          echo "=== Build Directory Contents ==="
          if [ -d "build" ]; then
            echo "Build directory structure:"
            ls -laR build/
            echo ""
            echo "=== WebGL Build Size ==="
            if [ -d "build/WebGL" ]; then
              du -sh build/WebGL
              echo ""
              echo "=== WebGL Contents ==="
              ls -lh build/WebGL/
            else
              echo "WebGL directory not found in build/"
            fi
          else
            echo "Build directory not found"
          fi
      # ビルド成果物のアップロード
      - name: Upload Build
        uses: actions/upload-artifact@v4
        if: always()
        with:
          name: WebGL-Build
          path: build/WebGL
          if-no-files-found: warn

      # GitHub Pagesへのデプロイ
      - name: Deploy to GitHub Pages
        if: success()
        uses: peaceiris/actions-gh-pages@v4
        with:
          github_token: ${{ secrets.GITHUB_TOKEN }}
          publish_dir: ./build/WebGL
          force_orphan: true

  notify:
    name: Notify Discord
    needs: build
    runs-on: ubuntu-latest
    if: always()
    
    steps:
      - name: Get short commit SHA
        id: sha
        run: echo "short_sha=$(echo ${{ github.sha }} | cut -c1-7)" >> $GITHUB_OUTPUT

      - name: Send Discord notification (Success)
        if: needs.build.result == 'success'
        uses: sarisia/actions-status-discord@v1
        with:
          webhook: ${{ secrets.DISCORD_WEBHOOK_URL }}
          status: success
          title: "🎮 Unity WebGL ビルド成功！"
          description: |
            **プロジェクト**: ${{ github.repository }}
            **ブランチ**: ${{ github.ref_name }}
            **コミット**: `${{ steps.sha.outputs.short_sha }}`
            
            🌐 **プレイ可能URL**: https://${{ github.repository_owner }}.github.io/${{ github.event.repository.name }}/
            
            コミットメッセージ: ${{ github.event.head_commit.message }}
          color: 0x00ff00
          username: Unity Build Bot
          avatar_url: https://unity.com/logo-unity-web.png

      - name: Send Discord notification (Failure)
        if: needs.build.result == 'failure'
        uses: sarisia/actions-status-discord@v1
        with:
          webhook: ${{ secrets.DISCORD_WEBHOOK_URL }}
          status: failure
          title: "❌ Unity WebGL ビルド失敗"
          description: |
            **プロジェクト**: ${{ github.repository }}
            **ブランチ**: ${{ github.ref_name }}
            **コミット**: `${{ steps.sha.outputs.short_sha }}`
            
            📋 **ログを確認**: https://github.com/${{ github.repository }}/actions/runs/${{ github.run_id }}
            
            コミットメッセージ: ${{ github.event.head_commit.message }}
          color: 0xff0000
          username: Unity Build Bot
          avatar_url: https://unity.com/logo-unity-web.png
