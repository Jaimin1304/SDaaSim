import subprocess
import sys
import os

current_script_path = os.path.dirname(os.path.abspath(__file__))
server_script_path = os.path.join(current_script_path, 'server.py')

# Determine the correct Python executable path
python_executable = sys.executable if sys.executable else "python"

# Set some environment variables to ensure the server runs in the background
server_env = os.environ.copy()
server_env["PYTHONUNBUFFERED"] = "1"

# Start the server as a background process
server_process = subprocess.Popen([python_executable, server_script_path], env=server_env)

# Keep the script running to keep the server process alive
server_process.communicate()
