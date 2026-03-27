const { app, BrowserWindow } = require('electron');
const { spawn } = require('child_process');
const path = require('path');
const http = require('http');

let backendProcess = null;

function startBackend() {
  return new Promise((resolve, reject) => {
    const backendDir = path.join(__dirname, '..', 'CSharpBackend');

    backendProcess = spawn('dotnet', ['run', '--project', backendDir], {
      stdio: ['ignore', 'pipe', 'pipe']
    });

    backendProcess.stdout.on('data', d => console.log('[backend]', d.toString()));
    backendProcess.stderr.on('data', d => console.error('[backend]', d.toString()));
    backendProcess.on('error', err => reject(err));

    // Poll until the C# API is ready
    const timeout = setTimeout(() => {
      clearInterval(poll);
      reject(new Error('C# backend failed to start within 30s'));
    }, 30000);

    const poll = setInterval(() => {
      http.get('http://localhost:5050/api/hello', res => {
        if (res.statusCode === 200) {
          clearInterval(poll);
          clearTimeout(timeout);
          resolve();
        }
      }).on('error', () => {}); // still starting up
    }, 500);
  });
}

async function createWindow() {
  await startBackend();

  const win = new BrowserWindow({
    width: 320,
    height: 220,
    resizable: false,
    title: 'Hello World App',
    webPreferences: {
      nodeIntegration: true,
      contextIsolation: false
    }
  });

  win.loadFile('index.html');
  win.setMenu(null);
}

app.whenReady().then(() => {
  createWindow().catch(err => {
    console.error(err);
    app.quit();
  });
});

app.on('window-all-closed', () => {
  if (backendProcess) backendProcess.kill();
  if (process.platform !== 'darwin') app.quit();
});

app.on('activate', () => {
  if (BrowserWindow.getAllWindows().length === 0) createWindow();
});
