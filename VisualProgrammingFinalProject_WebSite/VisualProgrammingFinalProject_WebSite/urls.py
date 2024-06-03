from django.urls import path
from app import views

urlpatterns = [
    path('login/', views.login_view, name='login'),
    path('students/', views.students, name='students'),
    
    path('parents/', views.parents, name='parents'),
    path('entry_exit_records/', views.entry_exit_records, name='entry_exit_records'),
    path('balance_management/', views.balance_management, name='balance_management'),
    path('transactions/', views.transactions, name='transactions'),
    path('restrictions/', views.restrictions, name='restrictions'),    

    path('logout/', views.logout_view, name='logout'),
    path('generate_school_qr_code/', views.generate_school_qr_code, name='generate_school_qr_code'),
    path('generate_purchase_qr_code/', views.generate_purchase_qr_code, name='generate_purchase_qr_code'),
    path('process_qr_code/<str:code_type>/', views.process_qr_code, name='process_qr_code'),
    path('', views.home, name='home'),
]
