import serial
import socket
import time


SERIAL_PORT = 'COM3'
BAUD_RATE = 115200 # Doit correspondre à la vitesse dans ton code ESP32 (ex: Serial.begin(9600))

SERVER_IP = '127.0.0.1'  # Ne pas toucher, c'est l'adresse locale du PC
TCP_PORT = 8051          # Ne pas toucher, sauf si ce port est déjà utilisé

# --- Initialisation et boucle ---
print("--- Serveur de pont Série vers TCP ---")

try:
    arduino = serial.Serial(SERIAL_PORT, BAUD_RATE, timeout=1)
    print(f"Port série {SERIAL_PORT} ouvert avec succès.")
except serial.SerialException as e:
    print(f"ERREUR: Impossible d'ouvrir le port série {SERIAL_PORT}.")
    print(f"   Détails: {e}")
    input("Appuyez sur Entrée pour quitter...")
    exit()

server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind((SERVER_IP, TCP_PORT))
server_socket.listen(1)

print(f"Serveur TCP en attente de connexion sur {SERVER_IP}:{TCP_PORT}...")
conn, addr = server_socket.accept()
print(f"Connexion établie avec le client {addr}")

try:
    with conn:
        while True:
            line = arduino.readline().decode('utf-8').strip()
            if line:
                conn.sendall((line + '\n').encode('utf-8'))
                print(f"-> Donnée envoyée: {line}")
            time.sleep(0.01)

except (ConnectionResetError, BrokenPipeError):
    print("Le client s'est déconnecté.")
except KeyboardInterrupt:
    print("Arrêt manuel du serveur.")
finally:
    arduino.close()
    server_socket.close()
    print("Serveur et port série fermés.")
    input("Appuyez sur Entrée pour quitter...")