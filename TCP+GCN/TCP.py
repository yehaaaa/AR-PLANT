import socket
import threading
import time
from GCN_train import GCN_start
from SOM import process_point_cloud  # import at the top
import os

menu_status_old = ""
menu_status_new = ""
roomNum = 0

client_sockets = []

host = '0.0.0.0' # Configure IP address, must be the same in UNITY
port = 2077 # Configure port, must be the same in UNITY

# Create TCP socket
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
# Bind server address and port
server_socket.bind((host, port))

# Listen for connections
server_socket.listen(10)
print("Server started, waiting for client connection...")
# File directory
path_origin = "D:\data\plant\UNITY-SOM"
# File name
name = "m82D_control1_B_D30_centre_filter.txt"
print(name)
# Intermediate storage for AR files
path_tem = "D:/data/modelnet40-save"
# Output location for GCN
path_gcn = "D:/data/modelnet40-GCN"
def broadcast_message(message):
    for client_socket in client_sockets:
        try:
            client_socket.send(message)
        except:
            client_sockets.remove(client_socket)


def handle_client(client_socket, client_address):
    global roomNum

    roomNum += 1
    client_name = roomNum
    print("name", client_name)
    print("client connected:", client_address)
    client_sockets.append(client_socket)
    global menu_status_old, menu_status_new
    menu_status_old = ""
    menu_status_new = ""
    t = '12345'
    b = '123456789'
    h = '\n'

    processed_file = os.path.join(path_origin, "processed_" + name, n_som=50, epochs=40) # n_som = 50, initial SOM nodes
    process_point_cloud(os.path.join(path_origin, name), processed_file)
    with open(processed_file, "rb") as file:
        print("Sending processed point cloud file...")
        while True:
            file_data = file.read(4096)
            if file_data:
                client_socket.send(file_data)
            else:
                time.sleep(2)
                client_socket.send(t.encode())
                break

    while True:
 
        data = client_socket.recv(1024)

        if not data:
            break

        if data.decode() == "file":
            print("Start receiving file")
            with open(os.path.join(path_tem, name), "wb") as file2:
                while True:
                    file2_data = client_socket.recv(1024 * 1024 * 6)

                    if file2_data.decode() == "OKOKOK":
                        print("Download complete")
                        GCN_start(os.path.join(path_tem, name), name)
                        break
                    if file2_data:
                        file2.write(file2_data)

        if data.decode() == "start-coloring":
            print("Start sending colored file")
            time.sleep(2)
            with open(os.path.join(path_gcn, name), "rb") as file1:
                while True:
                    file1_data = file1.read(4096)
                    if file1_data:
                        client_socket.send(file1_data)
                    else:
                        time.sleep(2)
                        print("Transfer complete")
                        client_socket.send(b.encode())
                        break

        # if data.decode().split(":")[0] == "menu":
        #     menu_status_new = data.decode()
        #     content = "menu"+str(client_name)+":"+menu_status_new.split("menu:")[1]
        #     if menu_status_old != menu_status_new:
        #         broadcast_message(content.encode()) 
        #         menu_status_old = menu_status_new
        #         print("position" + menu_status_old.split(":")[1])
        #         print("rotation" + menu_status_old.split(":")[2])
        #         print("scale" + menu_status_old.split(":")[3])

        if not data:
            break

    client_sockets.remove(client_socket)
    client_socket.close()
    print("Connection closed:", client_address)


while True:
    
    client_socket, client_address = server_socket.accept()
    
    client_thread = threading.Thread(target=handle_client, args=(client_socket, client_address))
    client_thread.start()
