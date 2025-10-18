
import {
    CCard, CCardBody,
    CContainer,
    CButton
} from '@coreui/react';

const Welcome = () => {
    return (
        <div className="animated fadeIn flex-row align-items-center">
            <CCard className="justify-content-center mb-2">
                <CCardBody className="pb-5">
                    <CContainer>
                        <h1 className="display-3">داشبورد انرژی!</h1>
                        <p className="lead">این برنامه با هدف تجزیه تحلیل و نمایش اطلاعات شبکه برق بصورت جدول و نمودار تک خطی و گرافهای آماری ایجاد شده است.</p>
                        <p> با استفاده از سامانه های سپاک و مدام مدیریت شبکه ، اطلاعات را از سایت مدیریت شبکه دریافت کرده و برای انجام تحلیل های آماری در بانک اطلاعاتی محلی ذخیره می شود .</p>
                        <p>برای استفاده از امکانات این سایت باید ابتدا با نام کاربری و رمز عبور خود وارد شوید.</p>
                        <p>در هنگام راه اندازی اولیه سایت کاربران نمونه در سه سطح مدیر ( Admins ) کاربران معمولی ( Users ) و کابران ویرایشگر ( Editors ) ایجاد می شود.</p>
                        <p>کاربران با نقش ( Admins ) به همه امکانات سایت از جمله تنظیمات اولیه دسترسی دارند.</p>
                        <p>کاربران با نقش ( Editors ) به امکانات ویرایشگری سایت دسترسی دارند اما امکان دسترسی به تنظیمات سایت را ندارند.</p>
                        <p>کاربران با نقش ( Users ) فقط امکان دیدن اطلاعات سایت را دارند و امکان ویرایش و تنظیمات سایت را ندارند.</p>

                        <hr className="my-2" />
                        <p className="lead">
                            <CButton color="primary">برای اطلاعات بیشتر با شرکت دانش بنیان پایش گستر پرمون تماس بگیرید</CButton>
                        </p>
                    </CContainer>
                </CCardBody>
            </CCard>
        </div>
    );
}

export { Welcome }
