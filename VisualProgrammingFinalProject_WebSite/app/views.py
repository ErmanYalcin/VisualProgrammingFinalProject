from django.http.response import JsonResponse
from django.shortcuts import render, redirect
from django.http import HttpResponse, FileResponse
import pyodbc
import qrcode
import io
from django.core.mail import send_mail
from django.utils import timezone

connection_string = (
    "DRIVER={ODBC Driver 17 for SQL Server};"
    "SERVER=ERMAN\\SQLEXPRESS;"
    "DATABASE=SchoolManagementSystem;"
    "Trusted_Connection=yes;"
)

def login_view(request):
    if request.method == 'POST':
        user_type = request.POST.get('user_type')
        username = request.POST.get('username')
        password = request.POST.get('password')

        conn = pyodbc.connect(connection_string)
        cursor = conn.cursor()

        if user_type == 'student':
            query = "SELECT * FROM Students WHERE SchoolNo = ? AND Password = ?"
            cursor.execute(query, (username, password))
        elif user_type == 'parent':
            query = "SELECT * FROM Parents WHERE Email = ? AND Password = ?"
            cursor.execute(query, (username, password))

        user = cursor.fetchone()
        conn.close()

        if user:
            request.session['user_type'] = user_type
            request.session['username'] = username
            if user_type == 'student':
                return redirect('students')
            elif user_type == 'parent':
                return redirect('parents')
        else:
            return render(request, 'login.html', {'error': 'Invalid login credentials'})

    return render(request, 'login.html')

def students(request):
    if request.session.get('user_type') != 'student':
        return redirect('login')
    
    conn = pyodbc.connect(connection_string)
    cursor = conn.cursor()
    query = "SELECT Balance FROM Students WHERE SchoolNo = ?"
    cursor.execute(query, (request.session.get('username'),))
    balance = cursor.fetchone()[0]

    query = """
    SELECT CafeteriaItems.ItemName FROM ParentRestrictions
    JOIN CafeteriaItems ON ParentRestrictions.RestrictedItemID = CafeteriaItems.ItemID
    WHERE ParentRestrictions.StudentID = (
        SELECT StudentID FROM Students WHERE SchoolNo = ?
    )
    """
    cursor.execute(query, (request.session.get('username'),))
    restrictions = [item[0] for item in cursor.fetchall()]

    conn.close()

    return render(request, 'students.html', {'balance': balance, 'restrictions': restrictions})

def home(request):
    return redirect('login')

def logout_view(request):
    request.session.flush()
    return redirect('login')

def generate_school_qr_code(request):
    if request.method == 'POST':
        username = request.session.get('username')
        current_time = timezone.now()
        report_content = f'The student with School No {username} has scanned the school QR code at {current_time}.'
        
        conn = pyodbc.connect(connection_string)
        cursor = conn.cursor()
        query = "INSERT INTO Reports (ReportType, GeneratedDate, ReportContent) VALUES (?, ?, ?)"
        cursor.execute(query, ('School Entry/Exit', current_time, report_content))
        conn.commit()
        
        parent_email = 'erman1103@icloud.com'
        send_mail(
            'School Entry/Exit Notification',
            report_content,
            'your_email@example.com',
            [parent_email],
            fail_silently=False,
        )
        
        conn.close()
        return JsonResponse({'message': 'School QR Code Processed'})
    
    return render(request, 'generate_school_qr_code.html')

def generate_purchase_qr_code(request):
    if request.method == 'POST':
        data = json.loads(request.body)
        item_id = data.get('item_id')
        quantity = data.get('quantity', 1)
        username = request.session.get('username')
        
        conn = pyodbc.connect(connection_string)
        cursor = conn.cursor()
        
        query = "SELECT Balance FROM Students WHERE SchoolNo = ?"
        cursor.execute(query, (username,))
        balance = cursor.fetchone()[0]
        
        query = "SELECT Price, Stock FROM CafeteriaItems WHERE ItemID = ?"
        cursor.execute(query, (item_id,))
        item = cursor.fetchone()
        item_price = item[0]
        item_stock = item[1]
        
        total_price = item_price * quantity
        
        if balance >= total_price and item_stock >= quantity:
            new_balance = balance - total_price
            new_stock = item_stock - quantity
            
            query = "UPDATE Students SET Balance = ? WHERE SchoolNo = ?"
            cursor.execute(query, (new_balance, username))
            conn.commit()
            
            query = "UPDATE CafeteriaItems SET Stock = ? WHERE ItemID = ?"
            cursor.execute(query, (new_stock, item_id))
            conn.commit()
            
            current_time = timezone.now()
            report_content = f'The student with School No {username} has made a purchase of {quantity} units of item {item_id} at {current_time}. New balance is {new_balance}.'
            query = "INSERT INTO Reports (ReportType, GeneratedDate, ReportContent) VALUES (?, ?, ?)"
            cursor.execute(query, ('Purchase', current_time, report_content))
            conn.commit()
            
            parent_email = 'erman1103@icloud.com'
            send_mail(
                'Purchase Notification',
                report_content,
                'your_email@example.com',
                [parent_email],
                fail_silently=False,
            )
            
            conn.close()
            return JsonResponse({'message': 'Purchase QR Code Processed'})
        else:
            conn.close()
            return JsonResponse({'message': 'Insufficient Balance or Stock'}, status=400)
    
    conn = pyodbc.connect(connection_string)
    cursor = conn.cursor()
    query = "SELECT ItemID, ItemName, Price, Stock FROM CafeteriaItems"
    cursor.execute(query)
    items = cursor.fetchall()
    conn.close()
    
    return render(request, 'generate_purchase_qr_code.html', {'items': items})

def process_qr_code(request, code_type):
    if request.method == 'POST':
        data = json.loads(request.body)
        item_id = data.get('item_id')
        quantity = data.get('quantity', 1)
        username = request.session.get('username')

        conn = pyodbc.connect(connection_string)
        cursor = conn.cursor()

        query = "SELECT Balance FROM Students WHERE SchoolNo = ?"
        cursor.execute(query, (username,))
        balance = cursor.fetchone()[0]

        query = "SELECT Price, Stock FROM CafeteriaItems WHERE ItemID = ?"
        cursor.execute(query, (item_id,))
        item = cursor.fetchone()
        item_price = item[0]
        item_stock = item[1]

        total_price = item_price * quantity

        if balance >= total_price and item_stock >= quantity:
            new_balance = balance - total_price
            new_stock = item_stock - quantity

            query = "UPDATE Students SET Balance = ? WHERE SchoolNo = ?"
            cursor.execute(query, (new_balance, username))
            conn.commit()

            query = "UPDATE CafeteriaItems SET Stock = ? WHERE ItemID = ?"
            cursor.execute(query, (new_stock, item_id))
            conn.commit()

            current_time = timezone.now()
            report_content = f'The student with School No {username} has made a purchase of {quantity} units of item {item_id} at {current_time}. New balance is {new_balance}.'
            query = "INSERT INTO Reports (ReportType, GeneratedDate, ReportContent) VALUES (?, ?, ?)"
            cursor.execute(query, ('Purchase', current_time, report_content))
            conn.commit()

            parent_email = 'erman1103@icloud.com'
            send_mail(
                'Purchase Notification',
                report_content,
                'your_email@example.com',
                [parent_email],
                fail_silently=False,
            )

            conn.close()
            return JsonResponse({'message': 'Purchase QR Code Processed'})
        else:
            conn.close()
            return JsonResponse({'message': 'Insufficient Balance or Stock'}, status=400)

def parents(request):
    if request.session.get('user_type') != 'parent':
        return redirect('login')

    conn = pyodbc.connect(connection_string)
    cursor = conn.cursor()
    query = "SELECT StudentID, FirstName, LastName, Balance FROM Students WHERE ParentID = (SELECT ParentID FROM Parents WHERE Email = ?)"
    cursor.execute(query, (request.session.get('username'),))
    students = cursor.fetchall()
    conn.close()

    return render(request, 'parents.html', {'students': students})

def entry_exit_records(request):
    if request.session.get('user_type') != 'parent':
        return redirect('login')
    
    conn = pyodbc.connect(connection_string)
    cursor = conn.cursor()
    query = "SELECT ReportType, GeneratedDate, ReportContent FROM Reports WHERE ReportType = 'School Entry/Exit' AND ReportContent LIKE ?"
    cursor.execute(query, (f'%{request.session.get("username")}%'))
    records = cursor.fetchall()
    conn.close()

    return render(request, 'entry_exit_records.html', {'records': records})

def balance_management(request):
    if request.session.get('user_type') != 'parent':
        return redirect('login')

    if request.method == 'POST':
        student_id = request.POST.get('student_id')
        amount = float(request.POST.get('amount'))
        action = request.POST.get('action')

        conn = pyodbc.connect(connection_string)
        cursor = conn.cursor()

        if action == 'Add Balance':
            query = "UPDATE Students SET Balance = Balance + ? WHERE StudentID = ?"
        else:
            query = "UPDATE Students SET Balance = Balance - ? WHERE StudentID = ?"

        cursor.execute(query, (amount, student_id))
        conn.commit()

    conn = pyodbc.connect(connection_string)
    cursor = conn.cursor()
    query = "SELECT StudentID, FirstName, LastName, Balance FROM Students WHERE ParentID = (SELECT ParentID FROM Parents WHERE Email = ?)"
    cursor.execute(query, (request.session.get('username'),))
    students = cursor.fetchall()
    conn.close()

    return render(request, 'balance_management.html', {'students': students})

def transactions(request):
    if request.session.get('user_type') != 'parent':
        return redirect('login')
    
    conn = pyodbc.connect(connection_string)
    cursor = conn.cursor()
    query = "SELECT ReportType, GeneratedDate, ReportContent FROM Reports WHERE ReportType = 'Purchase' AND ReportContent LIKE ?"
    cursor.execute(query, (f'%{request.session.get("username")}%'))
    transactions = cursor.fetchall()
    conn.close()

    return render(request, 'transactions.html', {'transactions': transactions})

def restrictions(request):
    if request.session.get('user_type') != 'parent':
        return redirect('login')

    if request.method == 'POST':
        student_id = request.POST.get('student_id')
        item_id = request.POST.get('item_id')
        action = request.POST.get('action')

        conn = pyodbc.connect(connection_string)
        cursor = conn.cursor()

        if action == 'Add Restriction':
            query = "INSERT INTO ParentRestrictions (StudentID, RestrictedItemID) VALUES (?, ?)"
        else:
            query = "DELETE FROM ParentRestrictions WHERE StudentID = ? AND RestrictedItemID = ?"

        cursor.execute(query, (student_id, item_id))
        conn.commit()

    conn = pyodbc.connect(connection_string)
    cursor = conn.cursor()
    query = "SELECT StudentID, FirstName, LastName FROM Students WHERE ParentID = (SELECT ParentID FROM Parents WHERE Email = ?)"
    cursor.execute(query, (request.session.get('username'),))
    students = cursor.fetchall()

    query = "SELECT ItemID, ItemName FROM CafeteriaItems"
    cursor.execute(query)
    items = cursor.fetchall()

    query = """
    SELECT Students.FirstName, Students.LastName, CafeteriaItems.ItemName FROM ParentRestrictions
    JOIN Students ON ParentRestrictions.StudentID = Students.StudentID
    JOIN CafeteriaItems ON ParentRestrictions.RestrictedItemID = CafeteriaItems.ItemID
    WHERE Students.ParentID = (SELECT ParentID FROM Parents WHERE Email = ?)
    """
    cursor.execute(query, (request.session.get('username'),))
    restrictions = cursor.fetchall()

    conn.close()

    return render(request, 'restrictions.html', {'students': students, 'items': items, 'restrictions': restrictions})
